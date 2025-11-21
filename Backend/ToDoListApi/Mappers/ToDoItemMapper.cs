using ToDoListApi.Domain.Models;
using ToDoListApi.Models;

namespace ToDoListApi.Mappers
{
    public static class ToDoItemMapper
    {
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
