namespace UserManagementAPI.Models.Requests
{
    public class CreateUserRequest
    {
        public bool IsActive { get; set; }
        public decimal? Balance { get; set; }
        public string PictureUrl { get; set; }
        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string FavoriteFruit { get; set; }
        public List<string> Tags { get; set; }
    }
}
