using static System.Net.Mime.MediaTypeNames;

namespace TodoApplication.Models.Entities
{
    public class Todo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public required string ActivitiesNo { get; set; }
        public required string Subject { get; set; }
        public string? Description { get; set; }
        // status => [null => unmarked, 1 => done, 0 => canceled]
        public int? Status { get; set; }
        public bool IsDeleted { get; set; } = false;

        public User? User { get; set; }
    }
}
