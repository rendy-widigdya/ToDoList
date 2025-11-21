using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Interfaces
{
    public interface IToDoListService
    {
        IEnumerable<ToDoItem> GetAll();
        ToDoItem? GetById(Guid id);
        ToDoItem Add(string title);
        bool Update(ToDoItem todo);
        bool Delete(Guid id);
    }
}
