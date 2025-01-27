using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models.Requests;
using UserManagementAPI.Models;

namespace UserManagementAPI.Endpoints.Users
{
    public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, bool>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserEndpoint(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override void Configure()
        {
            Put("/api/users/update/{id}");
            AllowAnonymous();
        }

        public override async Task<bool> ExecuteAsync(UpdateUserRequest req, CancellationToken ct)
        {
            var user = new User
            {
                Id = req.Id,
                IsActive = req.IsActive,
                Balance = req.Balance,
                PictureUrl = req.PictureUrl,
                Age = req.Age,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Company = req.Company,
                Email = req.Email,
                Address = req.Address,
                FavoriteFruit = req.FavoriteFruit,
                Tags = req.Tags
            };

            var success = await _userRepository.UpdateAsync(user);
            if (!success)
            {
                ThrowError("User not found or update failed", 404);
            }
            return success;
        }
    }
}
