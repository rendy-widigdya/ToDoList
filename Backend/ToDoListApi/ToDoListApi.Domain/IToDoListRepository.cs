using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoListApi.Domain
{
    public interface IToDoListRepository
    {
        IEnumerable<Todo> GetAll();
        Todo Add(Todo todo);
        bool Delete(Guid id);
        bool Update(Todo todo);
    }
}
