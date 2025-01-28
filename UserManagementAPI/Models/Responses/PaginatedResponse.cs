﻿namespace UserManagementAPI.Models.Responses
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
