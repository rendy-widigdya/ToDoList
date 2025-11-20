using ToDoListApi.Domain.Models;
using ToDoListApi.Models;

namespace ToDoListApi.Mappers
{
    public static class ToDoItemMapper
    {
        public static ToDoItem ToDomain(ToDoItemRequest request, Guid id = default)
        {
            return new ToDoItem
            {
                Id = id == default ? Guid.NewGuid() : id,
                Title = request.Title,
                IsDone = false,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public static ToDoItemResponse ToResponse(ToDoItem item)
        {
            return new ToDoItemResponse
            {
                Id = item.Id,
                Title = item.Title,
                IsDone = item.IsDone
            };
        }
    }
}
