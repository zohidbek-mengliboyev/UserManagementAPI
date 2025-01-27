using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models.Requests;
using UserManagementAPI.Models;

namespace UserManagementAPI.Endpoints.Users
{
    public class CreateUserEndpoint : Endpoint<CreateUserRequest, string>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserEndpoint(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override void Configure()
        {
            Post("/api/users/create");
            AllowAnonymous();
        }

        public override async Task<string> ExecuteAsync(CreateUserRequest req, CancellationToken ct)
        {
            var user = new User
            {
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

            return await _userRepository.CreateAsync(user);
        }
    }
}
