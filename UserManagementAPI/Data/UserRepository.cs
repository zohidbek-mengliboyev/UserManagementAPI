using Dapper;
using NanoidDotNet;
using Npgsql;
using System.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Specifications;

namespace UserManagementAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        private async Task EnsureConnectionOpenAsync()
        {
            if (_db.State != ConnectionState.Open)
            {
                await (_db as NpgsqlConnection)!.OpenAsync();
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            await EnsureConnectionOpenAsync();

            const string sql = @"
            SELECT u.*, string_agg(t.name, ',') as Tags
            FROM users u
            LEFT JOIN user_tags ut ON u.id = ut.user_id
            LEFT JOIN tags t ON ut.tag_id = t.id
            WHERE u.id = @Id
            GROUP BY u.id";

            var dto = await _db.QuerySingleOrDefaultAsync<UserDTO>(sql, new { Id = id });

            if (dto == null) return null;

            return new User
            {
                Id = dto.Id,
                IsActive = dto.IsActive,
                Balance = dto.Balance,
                PictureUrl = dto.PictureUrl,
                Age = dto.Age,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company,
                Email = dto.Email,
                Address = dto.Address,
                FavoriteFruit = dto.FavoriteFruit,
                Tags = dto.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public async Task<IEnumerable<User>> GetAllAsync(ISpecification<User> specification, int page, int pageSize)
        {
            await EnsureConnectionOpenAsync();

            var filter = specification?.ToSql() ?? string.Empty;
            var sort = specification?.GetOrderBy() ?? string.Empty;

            var sql = $@"
            SELECT u.*, string_agg(t.name, ',') as Tags
            FROM users u
            LEFT JOIN user_tags ut ON u.id = ut.user_id
            LEFT JOIN tags t ON ut.tag_id = t.id
            {(string.IsNullOrWhiteSpace(filter) ? "" : $"WHERE {filter}")}
            GROUP BY u.id
            {(sort.Contains("ORDER BY") ? sort : "ORDER BY u.created_at DESC")}
            OFFSET @Offset LIMIT @Limit";

            var parameters = specification?.GetParameters() ?? new Dictionary<string, object>();
            parameters["Offset"] = (page - 1) * pageSize;
            parameters["Limit"] = pageSize;

            var dtos = await _db.QueryAsync<UserDTO>(sql, parameters);

            if (!dtos.Any()) return Enumerable.Empty<User>();

            return dtos.Select(dto => new User
            {
                Id = dto.Id,
                IsActive = dto.IsActive,
                Balance = dto.Balance,
                PictureUrl = dto.PictureUrl,
                Age = dto.Age,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company,
                Email = dto.Email,
                Address = dto.Address,
                FavoriteFruit = dto.FavoriteFruit,
                Tags = dto.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            });
        }

        public async Task<int> CountAsync(UserSpecification specification)
        {
            await EnsureConnectionOpenAsync();

            var sql = @"SELECT COUNT(*) FROM users u";

            var conditions = specification.ToSql();
            if (!string.IsNullOrEmpty(conditions))
            {
                sql += $" WHERE {conditions}";
            }

            var parameters = specification.GetParameters();

            return await _db.ExecuteScalarAsync<int>(sql, parameters);
        }

        public async Task<string> CreateAsync(User user)
        {
            await EnsureConnectionOpenAsync();

            user.Id = Nanoid.Generate(size: 24);

            using var transaction = _db.BeginTransaction();
            try
            {
                const string insertUserSql = @"
                INSERT INTO users (id, is_active, balance, picture_url, age, first_name, last_name, 
                                 company, email, address, favorite_fruit)
                VALUES (@Id, @IsActive, @Balance, @PictureUrl, @Age, @FirstName, @LastName,
                        @Company, @Email, @Address, @FavoriteFruit)";

                await _db.ExecuteAsync(insertUserSql, user, transaction);

                if (user.Tags?.Any() == true)
                {
                    foreach (var tag in user.Tags)
                    {
                        const string insertTagSql = @"
                        INSERT INTO tags (name)
                        VALUES (@Name)
                        ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name
                        RETURNING id";

                        var tagId = await _db.QuerySingleAsync<int>(insertTagSql, new { Name = tag }, transaction);

                        const string insertUserTagSql = @"
                        INSERT INTO user_tags (user_id, tag_id)
                        VALUES (@UserId, @TagId)";

                        await _db.ExecuteAsync(insertUserTagSql, new { UserId = user.Id, TagId = tagId }, transaction);
                    }
                }

                transaction.Commit();
                return user.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            await EnsureConnectionOpenAsync();

            using var transaction = _db.BeginTransaction();
            try
            {
                const string updateUserSql = @"
                UPDATE users 
                SET is_active = @IsActive, balance = @Balance, picture_url = @PictureUrl,
                    age = @Age, first_name = @FirstName, last_name = @LastName,
                    company = @Company, email = @Email, address = @Address,
                    favorite_fruit = @FavoriteFruit
                WHERE id = @Id";

                var rowsAffected = await _db.ExecuteAsync(updateUserSql, user, transaction);

                if (rowsAffected > 0 && user.Tags?.Any() == true)
                {
                    const string deleteOldTagsSql = "DELETE FROM user_tags WHERE user_id = @UserId";
                    await _db.ExecuteAsync(deleteOldTagsSql, new { UserId = user.Id }, transaction);

                    foreach (var tag in user.Tags)
                    {
                        const string insertTagSql = @"
                        INSERT INTO tags (name)
                        VALUES (@Name)
                        ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name
                        RETURNING id";

                        var tagId = await _db.QuerySingleAsync<int>(insertTagSql, new { Name = tag }, transaction);

                        const string insertUserTagSql = @"
                        INSERT INTO user_tags (user_id, tag_id)
                        VALUES (@UserId, @TagId)";

                        await _db.ExecuteAsync(insertUserTagSql, new { UserId = user.Id, TagId = tagId }, transaction);
                    }
                }

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await EnsureConnectionOpenAsync();

            const string sql = "DELETE FROM users WHERE id = @Id";
            var rowsAffected = await _db.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
