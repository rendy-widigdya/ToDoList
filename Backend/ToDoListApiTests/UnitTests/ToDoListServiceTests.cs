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
        public void Add_WithTitleExceeding500Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longTitle = new string('a', 501);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Add(longTitle));
        }

        [Fact]
        public void Add_WithTitleExactly500Characters_ShouldSucceed()
        {
            // Arrange
            var title = new string('a', 500);
            var expectedTodo = new ToDoItem { Id = Guid.NewGuid(), Title = title };
            _repositoryMock.Setup(r => r.Add(It.IsAny<ToDoItem>())).Returns(expectedTodo);

            // Act
            var result = _service.Add(title);

            // Assert
            Assert.Equal(title, result.Title);
            _repositoryMock.Verify(r => r.Add(It.IsAny<ToDoItem>()), Times.Once);
        }

        [Fact]
        public void Add_WithWhitespaceTitle_ShouldTrimAndThrowArgumentException()
        {
            // Act & Assert - After trimming, it becomes empty
            Assert.Throws<ArgumentException>(() => _service.Add("   "));
        }

        [Fact]
        public void Add_WithTitleWithLeadingTrailingWhitespace_ShouldTrim()
        {
            // Arrange
            var title = "  Test Title  ";
            var expectedTodo = new ToDoItem { Id = Guid.NewGuid(), Title = "Test Title" };
            _repositoryMock.Setup(r => r.Add(It.IsAny<ToDoItem>())).Returns(expectedTodo);

            // Act
            var result = _service.Add(title);

            // Assert
            _repositoryMock.Verify(r => r.Add(It.Is<ToDoItem>(t => t.Title == "Test Title")), Times.Once);
        }

        [Fact]
        public void GetAll_ShouldReturnItemsOrderedByCreatedAt()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var todos = new List<ToDoItem>
            {
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 2", CreatedAt = now.AddMinutes(2) },
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 1", CreatedAt = now.AddMinutes(1) },
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 3", CreatedAt = now.AddMinutes(3) }
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(todos);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Task 1", result[0].Title);
            Assert.Equal("Task 2", result[1].Title);
            Assert.Equal("Task 3", result[2].Title);
        }

        [Fact]
        public void Update_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todo = new ToDoItem { Id = Guid.NewGuid(), Title = "" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Update(todo));
        }

        [Fact]
        public void Update_WithWhitespaceTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todo = new ToDoItem { Id = Guid.NewGuid(), Title = "   " };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Update(todo));
        }

        [Fact]
        public void Update_WithTitleExceeding500Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longTitle = new string('a', 501);
            var todo = new ToDoItem { Id = Guid.NewGuid(), Title = longTitle };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Update(todo));
        }

        [Fact]
        public void Update_WithTitleWithWhitespace_ShouldTrim()
        {
            // Arrange
            var todo = new ToDoItem { Id = Guid.NewGuid(), Title = "  Updated Title  ", IsDone = false, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.Update(It.IsAny<ToDoItem>())).Returns(true);

            // Act
            var result = _service.Update(todo);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Update(It.Is<ToDoItem>(t => t.Title == "Updated Title")), Times.Once);
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
