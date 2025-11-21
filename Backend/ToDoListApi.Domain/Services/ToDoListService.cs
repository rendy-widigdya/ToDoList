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
            // Domain layer applies business rules: default sort by creation date (oldest first)
            return _repository.GetAll().OrderBy(t => t.CreatedAt);
        }

        public ToDoItem? GetById(Guid id)
        {
            _logger.LogInformation("Retrieving todo item with ID: {Id}", id);
            return _repository.GetById(id);
        }

        public ToDoItem Add(string title)
        {
            var validatedTitle = ValidateAndTrimTitle(title, "add");

            _logger.LogInformation("Adding new todo item with title: {Title}", validatedTitle);
            
            // Domain layer owns entity creation and lifecycle
            var todo = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = validatedTitle,
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };
            
            var added = _repository.Add(todo);
            _logger.LogInformation("Successfully added todo item with ID: {Id}", added.Id);
            return added;
        }

        public bool Update(ToDoItem todo)
        {
            var validatedTitle = ValidateAndTrimTitle(todo.Title, "update");

            _logger.LogInformation("Updating todo item with ID: {Id}", todo.Id);
            
            // Create updated entity with validated and trimmed title
            var updatedTodo = todo with { Title = validatedTitle };
            var result = _repository.Update(updatedTodo);
            
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

        /// <summary>
        /// Validates and trims the title according to business rules.
        /// </summary>
        /// <param name="title">The title to validate</param>
        /// <param name="operation">The operation being performed (for logging purposes)</param>
        /// <returns>The validated and trimmed title</returns>
        /// <exception cref="ArgumentException">Thrown when title is invalid</exception>
        private string ValidateAndTrimTitle(string? title, string operation)
        {
            var trimmedTitle = title?.Trim();
            
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                _logger.LogWarning("Attempted to {Operation} todo item with empty title", operation);
                throw new ArgumentException("Title is required");
            }

            if (trimmedTitle.Length > 500)
            {
                _logger.LogWarning("Attempted to {Operation} todo item with title exceeding 500 characters", operation);
                throw new ArgumentException("Title cannot exceed 500 characters");
            }

            return trimmedTitle;
        }
    }
}
