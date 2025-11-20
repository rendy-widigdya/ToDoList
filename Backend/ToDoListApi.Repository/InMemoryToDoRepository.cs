using System.Collections.Concurrent;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;

namespace ToDoListApi.Infrastructure
{
    public class InMemoryToDoRepository : IToDoListRepository
    {
        private readonly ConcurrentDictionary<Guid, ToDoItem> _store = new();

        public IEnumerable<ToDoItem> GetAll() =>
            _store.Values.OrderBy(t => t.CreatedAt);

        public ToDoItem? GetById(Guid id) =>
            _store.TryGetValue(id, out var item) ? item : null;

        public ToDoItem Add(ToDoItem todo)
        {
            todo.Id = Guid.NewGuid();
            todo.CreatedAt = DateTime.UtcNow;
            _store[todo.Id] = todo;
            return todo;
        }

        public bool Delete(Guid id) => _store.TryRemove(id, out _);

        public bool Update(ToDoItem todo)
        {
            if (!_store.ContainsKey(todo.Id)) return false;
            _store[todo.Id] = todo;
            return true;
        }
    }
}
