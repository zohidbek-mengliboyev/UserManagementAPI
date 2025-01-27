using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<string> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
    }
}
