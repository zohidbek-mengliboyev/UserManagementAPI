using UserManagementAPI.Models;

namespace UserManagementAPI.Specifications
{
    public class UserSpecification : Specification<User>
    {
        public void AddFilterByName(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Conditions.Add("LOWER(u.first_name) LIKE LOWER(@Name) OR LOWER(u.last_name) LIKE LOWER(@Name)");
                Parameters["Name"] = $"%{name}%";
            }
        }

        public void AddFilterByAgeRange(int? minAge, int? maxAge)
        {
            if (minAge.HasValue)
            {
                Conditions.Add("u.age >= @MinAge");
                Parameters["MinAge"] = minAge.Value;
            }

            if (maxAge.HasValue)
            {
                Conditions.Add("u.age <= @MaxAge");
                Parameters["MaxAge"] = maxAge.Value;
            }
        }

        public void AddSorting(string? sortBy, bool? isDescending)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var direction = isDescending.HasValue ? (isDescending.Value == true ? "DESC" : "ASC") : "DESC";
                Sorting = $"ORDER BY u.{sortBy} {direction}";
            }
        }
    }
}
