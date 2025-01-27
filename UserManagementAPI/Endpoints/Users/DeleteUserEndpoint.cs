using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models.Requests;

namespace UserManagementAPI.Endpoints.Users
{
    public class DeleteUserEndpoint : Endpoint<UserRequest, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserEndpoint(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override void Configure()
        {
            Delete("/api/users/delete/{id}");
            AllowAnonymous();
        }

        public override async Task<bool> ExecuteAsync(UserRequest request, CancellationToken ct)
        {
            var success = await _userRepository.DeleteAsync(request.Id);
            if (!success)
            {
                ThrowError("User not found", 404);
            }
            return success;
        }
    }
}
