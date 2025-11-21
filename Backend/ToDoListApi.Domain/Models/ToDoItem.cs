namespace ToDoListApi.Domain.Models
{
    public record ToDoItem
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public bool IsDone { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
