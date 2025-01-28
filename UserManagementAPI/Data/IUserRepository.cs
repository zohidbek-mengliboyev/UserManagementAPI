using UserManagementAPI.Models;
using UserManagementAPI.Specifications;

namespace UserManagementAPI.Data
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync(ISpecification<User> specification, int page, int pageSize);
        Task<int> CountAsync(UserSpecification specification);
        Task<string> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
    }
}
