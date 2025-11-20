using Microsoft.Extensions.Logging;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;

namespace ToDoListApi.Domain.Services
{

    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _repository;
        private readonly ILogger<ToDoListService> _logger;

        public ToDoListService(IToDoListRepository repository, ILogger<ToDoListService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IEnumerable<ToDoItem> GetAll()
        {
            _logger.LogInformation("Retrieving all todo items");
            return _repository.GetAll();
        }

        public ToDoItem? GetById(Guid id)
        {
            _logger.LogInformation("Retrieving todo item with ID: {Id}", id);
            return _repository.GetById(id);
        }

        public ToDoItem Add(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("Attempted to add todo item with empty title");
                throw new ArgumentException("Title is required");
            }

            _logger.LogInformation("Adding new todo item with title: {Title}", title);
            var todo = new ToDoItem { Title = title };
            var added = _repository.Add(todo);
            _logger.LogInformation("Successfully added todo item with ID: {Id}", added.Id);
            return added;
        }

        public bool Update(ToDoItem todo)
        {
            _logger.LogInformation("Updating todo item with ID: {Id}", todo.Id);
            var result = _repository.Update(todo);
            if (result)
            {
                _logger.LogInformation("Successfully updated todo item with ID: {Id}", todo.Id);
            }
            else
            {
                _logger.LogWarning("Failed to update todo item with ID: {Id} - item not found", todo.Id);
            }
            return result;
        }

        public bool Delete(Guid id)
        {
            _logger.LogInformation("Deleting todo item with ID: {Id}", id);
            var result = _repository.Delete(id);
            if (result)
            {
                _logger.LogInformation("Successfully deleted todo item with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete todo item with ID: {Id} - item not found", id);
            }
            return result;
        }
    }
}
