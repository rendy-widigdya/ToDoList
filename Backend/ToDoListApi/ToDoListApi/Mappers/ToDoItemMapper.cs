using ToDoListApi.Domain.Models;
using ToDoListApi.Models;

namespace ToDoListApi.Domain.Mappers
{
    public static class ToDoItemMapper
    {
        public static ToDoItem ToDomain(ToDoItemRequest request)
        {
            return new ToDoItem
            {
                Id = Guid.NewGuid(),
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
                IsCompleted = item.IsDone
            };
        }
    }
}
