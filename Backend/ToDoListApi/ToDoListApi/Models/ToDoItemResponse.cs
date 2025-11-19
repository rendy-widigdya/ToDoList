namespace ToDoListApi.Models
{
    public class ToDoItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
