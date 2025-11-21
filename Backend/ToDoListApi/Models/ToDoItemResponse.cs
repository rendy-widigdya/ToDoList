namespace ToDoListApi.Models
{
    public record ToDoItemResponse
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public bool IsDone { get; init; }
    }
}
