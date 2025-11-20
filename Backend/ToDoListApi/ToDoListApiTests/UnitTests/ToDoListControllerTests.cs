using Moq;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;
using ToDoListApi.Domain.Services;
using ToDoListApi.Infrastructure;
using ToDoListApi.Domain.Mappers;
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
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<ToDoItem>>(result.Value);
            Assert.Equal(2, returnedTodos.Count());
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
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<ToDoItem>>(result.Value);
            Assert.Empty(returnedTodos);
        }

        [Fact]
        public void Create_WithValidTodo_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new ToDoItem { Title = "New Task" };
            var createdTodo = new ToDoItem { Id = Guid.NewGuid(), Title = "New Task" };
            _serviceMock.Setup(s => s.Add("New Task")).Returns(createdTodo);

            // Act
            var result = _controller.Create(request) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(nameof(ToDoListController.GetAll), result.ActionName);
            Assert.Equal(createdTodo, result.Value);
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ToDoItem { Title = "" };
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
            var request = new ToDoItem { Title = "Test" };
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
            var todo = new ToDoItem { Id = id, Title = "Updated Task" };
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>())).Returns(true);

            // Act
            var result = _controller.Update(id, todo);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todo = new ToDoItem { Id = id, Title = "Updated Task" };
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>())).Returns(false);

            // Act
            var result = _controller.Update(id, todo);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_ShouldSetIdFromRoute()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todo = new ToDoItem { Title = "Updated Task" };
            _serviceMock.Setup(s => s.Update(It.IsAny<ToDoItem>())).Returns(true);

            // Act
            var result = _controller.Update(id, todo);

            // Assert
            Assert.Equal(id, todo.Id);
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
