namespace TodoApplication.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }

        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
}
