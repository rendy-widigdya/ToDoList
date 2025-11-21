using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Models
{
    public record ToDoItemRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public required string Title { get; init; }
        public bool IsDone { get; init; } = false;
    }
}
