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
            var todo = new ToDoItem { Title = "Test Todo" };

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
            var todo = new ToDoItem { Title = "Test Todo" };
            var beforeAdd = DateTime.UtcNow;

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
            var todo1 = new ToDoItem { Title = "Todo 1" };
            var todo2 = new ToDoItem { Title = "Todo 2" };
            var todo3 = new ToDoItem { Title = "Todo 3" };

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
            var todo = _repository.Add(new ToDoItem { Title = "To Delete" });

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
            var todo = _repository.Add(new ToDoItem { Title = "Original Title" });
            todo.Title = "Updated Title";
            todo.IsDone = true;

            // Act
            var result = _repository.Update(todo);

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
            var nonExistingTodo = new ToDoItem { Id = Guid.NewGuid(), Title = "Non Existing" };

            // Act
            var result = _repository.Update(nonExistingTodo);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAll_ShouldReturnItemsOrderedByCreatedAt()
        {
            // Arrange
            var todo1 = _repository.Add(new ToDoItem { Title = "First" });
            System.Threading.Thread.Sleep(10);
            var todo2 = _repository.Add(new ToDoItem { Title = "Second" });

            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(todo1.Id, result[0].Id);
            Assert.Equal(todo2.Id, result[1].Id);
        }
    }
}
