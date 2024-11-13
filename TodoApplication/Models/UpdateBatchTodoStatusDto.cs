namespace TodoApplication.Models
{
    public class UpdateBatchTodoStatusDto
    {
        public List<Guid> Ids { get; set; }
        public int Status { get; set; }
    }
}
