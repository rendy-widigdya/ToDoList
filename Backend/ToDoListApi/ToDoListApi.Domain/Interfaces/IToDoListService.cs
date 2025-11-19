using System;
using System.Collections.Generic;
using System.Text;
using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Interfaces
{

    public interface IToDoListService
    {
        IEnumerable<ToDoItem> GetAll();
        ToDoItem Add(string title);
        bool Update(ToDoItem todo);
        bool Delete(Guid id);
    }
}
