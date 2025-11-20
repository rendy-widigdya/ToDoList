namespace ToDoListApi.Domain.Models
{
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public required string Title { get; set; } 
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
