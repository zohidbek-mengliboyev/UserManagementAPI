namespace UserManagementAPI.Models.Requests
{
    public class GetPaginatedUsersRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
