namespace UserManagementAPI.Models.Requests
{
    public class GetFilteredUsersRequest
    {
        public string? Name { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
