using Moq;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;
using ToDoListApi.Models;
using ToDoListApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListApiTests.UnitTests
{
    /// <summary>
    /// Unit tests for ToDoListController class.
    /// </summary>
    public class ToDoListControllerTests
    {
        private readonly Mock<IToDoListService> _serviceMock;
        private readonly ToDoListController _controller;

        public ToDoListControllerTests()
        {
            _serviceMock = new Mock<IToDoListService>();
            _controller = new ToDoListController(_serviceMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithTodos()
        {
            // Arrange
            var todos = new List<ToDoItem>
            {
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 1" },
                new ToDoItem { Id = Guid.NewGuid(), Title = "Task 2" }
            };
            _serviceMock.Setup(s => s.GetAll()).Returns(todos);

            // Act
            var result = _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<ToDoItemResponse>>(result.Value);
            Assert.Equal(2, returnedTodos.Count());
            Assert.Contains(returnedTodos, t => t.Title == "Task 1");
            Assert.Contains(returnedTodos, t => t.Title == "Task 2");
        }

        [Fact]
        public void GetAll_WhenEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAll()).Returns(new List<ToDoItem>());

            // Act
            var result = _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<ToDoItemResponse>>(result.Value);
            Assert.Empty(returnedTodos);
        }

        [Fact]
        public void Create_WithValidTodo_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new ToDoItemRequest { Title = "New Task" };
            var createdTodo = new ToDoItem { Id = Guid.NewGuid(), Title = "New Task" };
            _serviceMock.Setup(s => s.Add("New Task")).Returns(createdTodo);

            // Act
            var result = _controller.Create(request) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(nameof(ToDoListController.GetAll), result.ActionName);
            var response = Assert.IsType<ToDoItemResponse>(result.Value);
            Assert.Equal("New Task", response.Title);
            Assert.Equal(createdTodo.Id, response.Id);
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ToDoItemRequest { Title = "" };
            _serviceMock.Setup(s => s.Add("")).Throws<ArgumentException>();

            // Act
            var result = _controller.Create(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void Create_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ToDoItemRequest { Title = "Test" };
            _serviceMock.Setup(s => s.Add("Test")).Throws(new ArgumentException("Title is required"));

            // Act
            var result = _controller.Create(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void Update_WithExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new ToDoItemRequest { Title = "Updated Task" };
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>())).Returns(true);

            // Act
            var result = _controller.Update(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new ToDoItemRequest { Title = "Updated Task" };
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>())).Returns(false);

            // Act
            var result = _controller.Update(id, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_ShouldSetIdFromRoute()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new ToDoItemRequest { Title = "Updated Task" };
            ToDoItem? captured = null;
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>()))
                .Callback<ToDoItem>(t => captured = t)
                .Returns(true);

            // Act
            var result = _controller.Update(id, request);

            // Assert
            Assert.Equal(id, captured!.Id);
            Assert.Equal("Updated Task", captured.Title);
            _serviceMock.Verify(s => s.Update(It.Is<ToDoItem>(t => t.Id == id)), Times.Once);
        }

        [Fact]
        public void Delete_WithExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.Delete(id)).Returns(true);

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.Delete(id)).Returns(false);

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ShouldCallServiceWithCorrectId()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.Delete(id)).Returns(true);

            // Act
            _controller.Delete(id);

            // Assert
            _serviceMock.Verify(s => s.Delete(id), Times.Once);
        }
    }
}
