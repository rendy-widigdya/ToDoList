using Moq;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;
using ToDoListApi.Domain.Services;
using Microsoft.Extensions.Logging;

namespace ToDoListApiTests.UnitTests
{
    /// <summary>
    /// Unit tests for ToDoListService class.
    /// </summary>
    public class ToDoListServiceTests
    {
        private readonly Mock<IToDoListRepository> _repositoryMock;
        private readonly Mock<ILogger<ToDoListService>> _loggerMock;
        private readonly ToDoListService _service;

        public ToDoListServiceTests()
        {
            _repositoryMock = new Mock<IToDoListRepository>();
            _loggerMock = new Mock<ILogger<ToDoListService>>();
            _service = new ToDoListService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Add_WithValidTitle_ShouldCallRepository()
        {
            // Arrange
            var title = "New Task";
            var expectedTodo = new ToDoItem { Id = Guid.NewGuid(), Title = title };
            _repositoryMock.Setup(r => r.Add(It.IsAny<ToDoItem>())).Returns(expectedTodo);

            // Act
            var result = _service.Add(title);

            // Assert
            Assert.Equal(title, result.Title);
            _repositoryMock.Verify(r => r.Add(It.Is<ToDoItem>(t => t.Title == title)), Times.Once);
        }

        [Fact]
        public void Add_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Add(""));
        }

        [Fact]
        public void Add_WithWhitespaceTitle_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Add("   "));
        }

        [Fact]
        public void Add_WithNullTitle_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Add(null));
        }

        [Fact]
        public void GetAll_ShouldReturnRepositoryItems()
        {
            // Arrange
            var todos = new List<ToDoItem>
            {
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 1" },
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 2" }
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(todos);

            // Act
            var result = _service.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
            _repositoryMock.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void Update_WithValidTodo_ShouldCallRepository()
        {
            // Arrange
            var todo = new ToDoItem { Id = Guid.NewGuid(), Title = "Updated Task" };
            _repositoryMock.Setup(r => r.Update(It.IsAny<ToDoItem>())).Returns(true);

            // Act
            var result = _service.Update(todo);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Update(todo), Times.Once);
        }

        [Fact]
        public void Delete_WithValidId_ShouldCallRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Delete(id)).Returns(true);

            // Act
            var result = _service.Delete(id);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.Delete(id)).Returns(false);

            // Act
            var result = _service.Delete(id);

            // Assert
            Assert.False(result);
        }
    }

}
