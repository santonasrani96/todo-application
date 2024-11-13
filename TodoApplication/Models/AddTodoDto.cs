namespace TodoApplication.Models
{
    public class AddTodoDto
    {
        public Guid UserId { get; set; }
        public required string Subject { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}
