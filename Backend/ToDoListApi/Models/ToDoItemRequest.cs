using System.ComponentModel.DataAnnotations;
using ToDoListApi.Validation;

namespace ToDoListApi.Models
{
    public record ToDoItemRequest
    {
        [TrimmedRequired]
        [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public required string Title { get; init; }
        public bool IsDone { get; init; } = false;
    }
}
