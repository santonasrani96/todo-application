namespace TodoApplication.Models
{
    public class LoginRequestDto
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
    }
}
