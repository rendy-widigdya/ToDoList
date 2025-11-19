using System;
using System.Collections.Generic;
using System.Text;
using ToDoListApi.Domain.Models;
using ToDoListApi.Models;

namespace ToDoListApi.Domain.Mappers
{
    public static class TodoItemMapper
    {
        public static ToDoItem ToDomain(ToDoItemRequest request)
        {
            return new ToDoItem
            {
                Title = request.Title,
                IsDone = false
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
