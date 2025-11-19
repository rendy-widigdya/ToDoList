using System.Collections.Concurrent;
using ToDoListApi.Domain;

namespace ToDoListApi.Repository
{
    public class InMemoryTodoRepository : IToDoListRepository
    {
        private readonly ConcurrentDictionary<Guid, Todo> _store = new();

        public IEnumerable<Todo> GetAll() =>
            _store.Values.OrderBy(t => t.CreatedAt);

        public Todo Add(Todo todo)
        {
            todo.Id = Guid.NewGuid();
            todo.CreatedAt = DateTime.UtcNow;
            _store[todo.Id] = todo;
            return todo;
        }

        public bool Delete(Guid id) => _store.TryRemove(id, out _);

        public bool Update(Todo todo)
        {
            if (!_store.ContainsKey(todo.Id)) return false;
            _store[todo.Id] = todo;
            return true;
        }
    }
}
