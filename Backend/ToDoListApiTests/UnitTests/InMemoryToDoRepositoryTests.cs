using ToDoListApi.Domain.Models;
using ToDoListApi.Infrastructure;

namespace ToDoListApiTests.UnitTests
{
    /// <summary>
    /// Unit tests for InMemoryTodoRepository class.
    /// </summary>
    public class InMemoryTodoRepositoryTests
    {
        private readonly InMemoryToDoRepository _repository;

        public InMemoryTodoRepositoryTests()
        {
            _repository = new InMemoryToDoRepository();
        }

        [Fact]
        public void GetAll_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var result = _repository.GetAll();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Add_ShouldAddTodoAndGenerateId()
        {
            // Arrange
            var todo = new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Test Todo",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var added = _repository.Add(todo);

            // Assert
            Assert.NotEqual(Guid.Empty, added.Id);
            Assert.Equal("Test Todo", added.Title);
            Assert.Single(_repository.GetAll());
        }

        [Fact]
        public void Add_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange
            var beforeAdd = DateTime.UtcNow;
            var todo = new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Test Todo",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var added = _repository.Add(todo);
            var afterAdd = DateTime.UtcNow;

            // Assert
            Assert.InRange(added.CreatedAt, beforeAdd, afterAdd);
        }

        [Fact]
        public void Add_MultipleTodos_ShouldAddAllItems()
        {
            // Arrange
            var todo1 = new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Todo 1",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };
            var todo2 = new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Todo 2",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };
            var todo3 = new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Todo 3",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            _repository.Add(todo1);
            _repository.Add(todo2);
            _repository.Add(todo3);

            // Assert
            Assert.Equal(3, _repository.GetAll().Count());
        }

        [Fact]
        public void Delete_ExistingTodo_ShouldReturnTrueAndRemove()
        {
            // Arrange
            var todo = _repository.Add(new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "To Delete",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            });

            // Act
            var result = _repository.Delete(todo.Id);

            // Assert
            Assert.True(result);
            Assert.Empty(_repository.GetAll());
        }

        [Fact]
        public void Delete_NonExistingTodo_ShouldReturnFalse()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = _repository.Delete(nonExistingId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Update_ExistingTodo_ShouldReturnTrueAndModify()
        {
            // Arrange
            var todo = _repository.Add(new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Original Title",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            });
            var updatedTodo = todo with 
            { 
                Title = "Updated Title",
                IsDone = true
            };

            // Act
            var result = _repository.Update(updatedTodo);

            // Assert
            Assert.True(result);
            var updated = _repository.GetAll().First(t => t.Id == todo.Id);
            Assert.Equal("Updated Title", updated.Title);
            Assert.True(updated.IsDone);
        }

        [Fact]
        public void Update_NonExistingTodo_ShouldReturnFalse()
        {
            // Arrange
            var nonExistingTodo = new ToDoItem 
            { 
                Id = Guid.NewGuid(), 
                Title = "Non Existing",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _repository.Update(nonExistingTodo);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            // Arrange
            var todo1 = _repository.Add(new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "First",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            });
            var todo2 = _repository.Add(new ToDoItem 
            { 
                Id = Guid.NewGuid(),
                Title = "Second",
                IsDone = false,
                CreatedAt = DateTime.UtcNow
            });

            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Contains(result, x => x.Id == todo1.Id);
            Assert.Contains(result, x => x.Id == todo2.Id);
        }
    }
}
