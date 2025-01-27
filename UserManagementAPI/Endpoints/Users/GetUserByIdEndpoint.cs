using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Models.Requests;

namespace UserManagementAPI.Endpoints.Users
{
    public class GetUserByIdEndpoint : Endpoint<UserRequest, User>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdEndpoint(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override void Configure()
        {
            Get("/api/users/get-by/{id}");
            AllowAnonymous();
        }

        public override async Task<User> ExecuteAsync(UserRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                ThrowError("User not found", 404);
            }
            return user;
        }
    }
}
