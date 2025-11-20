using ToDoListApi.Domain.Models;
using ToDoListApi.Mappers;
using ToDoListApi.Models;

namespace ToDoListApiTests.UnitTests
{
    /// <summary>
    /// Unit tests for TodoItemMapper class.
    /// </summary>
    public class ToDoItemMapperTests
    {
        [Fact]
        public void ToDomain_WithValidRequest_ShouldMapToToDoItem()
        {
            // Arrange
            var request = new ToDoItemRequest { Title = "Test Task" };

            // Act
            var result = ToDoItemMapper.ToDomain(request);

            // Assert
            Assert.Equal("Test Task", result.Title);
            Assert.False(result.IsDone);
        }

        [Fact]
        public void ToDomain_ShouldSetIsDoneToFalse()
        {
            // Arrange
            var request = new ToDoItemRequest { Title = "Test" };

            // Act
            var result = ToDoItemMapper.ToDomain(request);

            // Assert
            Assert.False(result.IsDone);
        }

        [Fact]
        public void ToResponse_WithValidToDoItem_ShouldMapToResponse()
        {
            // Arrange
            var item = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                IsDone = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = ToDoItemMapper.ToResponse(item);

            // Assert
            Assert.Equal(item.Id, result.Id);
            Assert.Equal("Test Task", result.Title);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public void ToResponse_MapsDoneToIsCompleted()
        {
            // Arrange
            var item = new ToDoItem { Title = "Task", IsDone = true };

            // Act
            var result = ToDoItemMapper.ToResponse(item);

            // Assert
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public void ToResponse_WithFalseDone_MapsToFalseIsCompleted()
        {
            // Arrange
            var item = new ToDoItem { Title = "Task", IsDone = false };

            // Act
            var result = ToDoItemMapper.ToResponse(item);

            // Assert
            Assert.False(result.IsCompleted);
        }
    }
}
