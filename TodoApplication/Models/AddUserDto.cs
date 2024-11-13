namespace TodoApplication.Models
{
    public class AddUserDto
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
    }
}
