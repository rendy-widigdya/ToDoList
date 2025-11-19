using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoListApi.Domain
{

    public interface IToDoListService
    {
        IEnumerable<Todo> GetAll();
        Todo Add(string title);
        bool Update(Todo todo);
        bool Delete(Guid id);
    }
}
