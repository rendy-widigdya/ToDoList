using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoListApi.Domain
{

    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _repository;

        public ToDoListService(IToDoListRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Todo> GetAll() => _repository.GetAll();

        public Todo Add(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            var todo = new Todo { Title = title };
            return _repository.Add(todo);
        }

        public bool Update(Todo todo) => _repository.Update(todo);

        public bool Delete(Guid id) => _repository.Delete(id);
    }
}
