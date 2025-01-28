using FastEndpoints;
using UserManagementAPI.Data;
using UserManagementAPI.Models.Requests;
using UserManagementAPI.Models;
using UserManagementAPI.Models.Responses;
using UserManagementAPI.Specifications;

namespace UserManagementAPI.Endpoints.Users
{
    public class GetAllUsersEndpoint : Endpoint<GetFilteredUsersRequest, PaginatedResponse<User>>
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

        public override async Task<PaginatedResponse<User>> ExecuteAsync(GetFilteredUsersRequest req, CancellationToken ct)
        {
            var spec = new UserSpecification();
            spec.AddFilterByName(req.Name);
            spec.AddFilterByAgeRange(req.MinAge, req.MaxAge);
            spec.AddSorting(req.SortBy, req.IsDescending);

            var count = await _userRepository.CountAsync(spec);

            const int maxPageSize = 100;
            req.PageSize = Math.Min(req.PageSize, maxPageSize); 
            
            var users = await _userRepository.GetAllAsync(spec, req.Page, req.PageSize);

            return new PaginatedResponse<User>
            {
                Data = users,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalCount = count
            };
        }
    }
}
