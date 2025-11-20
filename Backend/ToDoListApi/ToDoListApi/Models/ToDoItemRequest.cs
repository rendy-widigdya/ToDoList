using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Models
{
    public class ToDoItemRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public required string Title { get; set; }
        public bool IsDone { get; set; } = false;
    }
}
