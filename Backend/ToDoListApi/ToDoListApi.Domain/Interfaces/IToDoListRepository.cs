using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Interfaces
{
    public interface IToDoListRepository
    {
        IEnumerable<ToDoItem> GetAll();
        ToDoItem? GetById(Guid id);
        ToDoItem Add(ToDoItem todo);
        bool Delete(Guid id);
        bool Update(ToDoItem todo);
    }
}
