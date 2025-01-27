namespace UserManagementAPI.Models.Requests
{
    public class UpdateUserRequest : CreateUserRequest
    {
        public string Id { get; set; }
    }
}
