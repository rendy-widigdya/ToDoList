namespace ToDoListApi.Models
{
    public class ToDoItemResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
