using System;
using System.Collections.Generic;
using System.Text;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Services
{

    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _repository;

        public ToDoListService(IToDoListRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ToDoItem> GetAll() => _repository.GetAll();

        public ToDoItem Add(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            var todo = new ToDoItem { Title = title };
            return _repository.Add(todo);
        }

        public bool Update(ToDoItem todo) => _repository.Update(todo);

        public bool Delete(Guid id) => _repository.Delete(id);
    }
}
