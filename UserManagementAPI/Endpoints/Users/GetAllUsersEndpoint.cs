using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models.Requests;
using UserManagementAPI.Models;

namespace UserManagementAPI.Endpoints.Users
{
    public class GetAllUsersEndpoint : Endpoint<GetPaginatedUsersRequest, IEnumerable<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersEndpoint(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override void Configure()
        {
            Get("/api/users/get-all");
            AllowAnonymous();
        }

        public override async Task<IEnumerable<User>> ExecuteAsync(GetPaginatedUsersRequest req, CancellationToken ct)
        {
            return await _userRepository.GetAllAsync(req.Page, req.PageSize);
        }
    }
}
