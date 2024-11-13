namespace TodoApplication.Models
{
    public class UpdateTodoStatusDto
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }
}
