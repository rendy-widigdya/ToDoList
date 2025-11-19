using System;
using System.Collections.Generic;
using System.Text;
using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Interfaces
{
    public interface IToDoListRepository
    {
        IEnumerable<ToDoItem> GetAll();
        ToDoItem Add(ToDoItem todo);
        bool Delete(Guid id);
        bool Update(ToDoItem todo);
    }
}
