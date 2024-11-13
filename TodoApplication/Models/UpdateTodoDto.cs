namespace TodoApplication.Models
{
    public class UpdateTodoDto
    {
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}
