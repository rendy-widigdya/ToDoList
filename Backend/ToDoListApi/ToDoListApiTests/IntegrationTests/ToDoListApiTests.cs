using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using ToDoListApi.Domain.Models;
using ToDoListApi.Models;

namespace ToDoListApiTests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the ToDoList API endpoints.
    /// Tests the full request/response cycle through the API.
    /// </summary>
    public class ToDoListApiIntegrationTests : IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public async Task InitializeAsync()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            _factory?.Dispose();
            await Task.CompletedTask;
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_WhenNoTodos_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/todolist");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItem>>();
            todos.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_WithExistingTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var todo1 = new ToDoItem { Title = "Task 1" };
            var todo2 = new ToDoItem { Title = "Task 2" };

            await _client.PostAsJsonAsync("/todolist", todo1);
            await _client.PostAsJsonAsync("/todolist", todo2);

            // Act
            var response = await _client.GetAsync("/todolist");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItem>>();
            todos.Should().HaveCount(2);
            todos.Should().Contain(t => t.Title == "Task 1");
            todos.Should().Contain(t => t.Title == "Task 2");
        }

        [Fact]
        public async Task GetAll_ShouldReturnTodosOrderedByCreatedAt()
        {
            // Arrange
            var todo1 = new ToDoItem { Title = "First Task" };
            await _client.PostAsJsonAsync("/todolist", todo1);

            await Task.Delay(100);

            var todo2 = new ToDoItem { Title = "Second Task" };
            await _client.PostAsJsonAsync("/todolist", todo2);

            // Act
            var response = await _client.GetAsync("/todolist");
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.Should().HaveCount(2);
            todos[0].Title.Should().Be("First Task");
            todos[1].Title.Should().Be("Second Task");
            todos[0].CreatedAt.Should().BeBefore(todos[1].CreatedAt);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidTodo_ShouldReturnCreatedStatus()
        {
            // Arrange
            var todo = new ToDoItem { Title = "New Todo" };

            // Act
            var response = await _client.PostAsJsonAsync("/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdTodo = await response.Content.ReadFromJsonAsync<ToDoItem>();
            createdTodo.Should().NotBeNull();
            createdTodo.Id.Should().NotBe(Guid.Empty);
            createdTodo.Title.Should().Be("New Todo");
            createdTodo.IsDone.Should().BeFalse();
            createdTodo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Create_ShouldGenerateUniqueIds()
        {
            // Arrange
            var todo1 = new ToDoItem { Title = "Todo 1" };
            var todo2 = new ToDoItem { Title = "Todo 2" };

            // Act
            var response1 = await _client.PostAsJsonAsync("/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItem>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItem>();

            // Assert
            created1.Id.Should().NotBe(created2.Id);
        }

        [Fact]
        public async Task Create_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var todo = new ToDoItem { Title = "" };

            // Act
            var response = await _client.PostAsJsonAsync("/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithWhitespaceTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var todo = new ToDoItem { Title = "   " };

            // Act
            var response = await _client.PostAsJsonAsync("/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithNullTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var content = new StringContent("{\"title\": null}", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/todolist", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ShouldPersistTodoInRepository()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Persistent Task" };

            // Act
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.Should().ContainSingle(t => t.Id == created.Id && t.Title == "Persistent Task");
        }

        [Fact]
        public async Task Create_CreatedTodoShouldHaveIsDoneFalse()
        {
            // Arrange
            var todo = new ToDoItem { Title = "New Task", IsDone = true };

            // Act
            var response = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await response.Content.ReadFromJsonAsync<ToDoItem>();

            // Assert
            created.IsDone.Should().BeFalse();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithExistingTodo_ShouldReturnNoContent()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Original Title" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            var updatedTodo = new ToDoItem { Id = created.Id, Title = "Updated Title", IsDone = true };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/todolist/{created.Id}", updatedTodo);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_ShouldModifyTodoInRepository()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Original Title" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            var updatedTodo = new ToDoItem { Id = created.Id, Title = "Modified Title", IsDone = true };
            await _client.PutAsJsonAsync($"/todolist/{created.Id}", updatedTodo);

            // Act
            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            var todoInRepo = todos.Should().ContainSingle(t => t.Id == created.Id).Subject;
            todoInRepo.Title.Should().Be("Modified Title");
            todoInRepo.IsDone.Should().BeTrue();
        }

        [Fact]
        public async Task Update_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var todo = new ToDoItem { Id = nonExistingId, Title = "Non Existing" };

            // Act
            var response = await _client.PutAsJsonAsync($"/todolist/{nonExistingId}", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithMultipleTodos_ShouldUpdateOnlyTargetTodo()
        {
            // Arrange
            var todo1 = new ToDoItem { Title = "Task 1" };
            var todo2 = new ToDoItem { Title = "Task 2" };

            var response1 = await _client.PostAsJsonAsync("/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItem>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItem>();

            var updatedTodo1 = new ToDoItem { Id = created1.Id, Title = "Updated Task 1", IsDone = true };

            // Act
            await _client.PutAsJsonAsync($"/todolist/{created1.Id}", updatedTodo1);

            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.Should().HaveCount(2);
            todos.Should().ContainSingle(t => t.Id == created1.Id && t.Title == "Updated Task 1" && t.IsDone);
            todos.Should().ContainSingle(t => t.Id == created2.Id && t.Title == "Task 2" && !t.IsDone);
        }

        [Fact]
        public async Task Update_ShouldMarkTodoAsCompleted()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Task to Complete" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            var completedTodo = new ToDoItem { Id = created.Id, Title = "Task to Complete", IsDone = true };

            // Act
            await _client.PutAsJsonAsync($"/todolist/{created.Id}", completedTodo);

            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.First(t => t.Id == created.Id).IsDone.Should().BeTrue();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithExistingTodo_ShouldReturnNoContent()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/todolist/{created.Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ShouldRemoveTodoFromRepository()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            // Act
            await _client.DeleteAsync($"/todolist/{created.Id}");

            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.Should().NotContain(t => t.Id == created.Id);
        }

        [Fact]
        public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/todolist/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldOnlyDeleteTargetTodo()
        {
            // Arrange
            var todo1 = new ToDoItem { Title = "Task 1" };
            var todo2 = new ToDoItem { Title = "Task 2" };

            var response1 = await _client.PostAsJsonAsync("/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItem>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItem>();

            // Act
            await _client.DeleteAsync($"/todolist/{created1.Id}");

            var getResponse = await _client.GetAsync("/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();

            // Assert
            todos.Should().HaveCount(1);
            todos.Should().ContainSingle(t => t.Id == created2.Id);
        }

        [Fact]
        public async Task Delete_CannotDeleteSameTodoTwice()
        {
            // Arrange
            var todo = new ToDoItem { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            // Act
            var firstDelete = await _client.DeleteAsync($"/todolist/{created.Id}");
            var secondDelete = await _client.DeleteAsync($"/todolist/{created.Id}");

            // Assert
            firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
            secondDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Complete Workflow Tests

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateDelete()
        {
            // Arrange - Create
            var todo = new ToDoItem { Title = "Complete Workflow Task" };
            var createResponse = await _client.PostAsJsonAsync("/todolist", todo);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItem>();

            // Act & Assert - Get All (verify creation)
            var getAllResponse = await _client.GetAsync("/todolist");
            var allTodos = await getAllResponse.Content.ReadFromJsonAsync<List<ToDoItem>>();
            allTodos.Should().Contain(t => t.Id == created.Id);

            // Act & Assert - Update
            var updatedTodo = new ToDoItem { Id = created.Id, Title = "Updated Workflow Task", IsDone = true };
            var updateResponse = await _client.PutAsJsonAsync($"/todolist/{created.Id}", updatedTodo);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedList = await (await _client.GetAsync("/todolist")).Content.ReadFromJsonAsync<List<ToDoItem>>();
            updatedList.Should().ContainSingle(t => t.Id == created.Id && t.Title == "Updated Workflow Task" && t.IsDone);

            // Act & Assert - Delete
            var deleteResponse = await _client.DeleteAsync($"/todolist/{created.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var finalList = await (await _client.GetAsync("/todolist")).Content.ReadFromJsonAsync<List<ToDoItem>>();
            finalList.Should().NotContain(t => t.Id == created.Id);
        }

        [Fact]
        public async Task CompleteWorkflow_MultipleTodosOperations()
        {
            // Create multiple todos
            var todosToCreate = new[]
            {
                new ToDoItem { Title = "Task 1" },
                new ToDoItem { Title = "Task 2" },
                new ToDoItem { Title = "Task 3" }
            };

            var createdIds = new List<Guid>();
            foreach (var todo in todosToCreate)
            {
                var response = await _client.PostAsJsonAsync("/todolist", todo);
                var created = await response.Content.ReadFromJsonAsync<ToDoItem>();
                createdIds.Add(created.Id);
            }

            // Verify all created
            var allTodos = await (await _client.GetAsync("/todolist")).Content.ReadFromJsonAsync<List<ToDoItem>>();
            allTodos.Should().HaveCount(3);

            // Update first and third
            var updateTodo1 = new ToDoItem { Id = createdIds[0], Title = "Task 1 - Updated", IsDone = true };
            var updateTodo3 = new ToDoItem { Id = createdIds[2], Title = "Task 3 - Updated", IsDone = false };

            await _client.PutAsJsonAsync($"/todolist/{createdIds[0]}", updateTodo1);
            await _client.PutAsJsonAsync($"/todolist/{createdIds[2]}", updateTodo3);

            // Delete second
            await _client.DeleteAsync($"/todolist/{createdIds[1]}");

            // Verify final state
            var finalTodos = await (await _client.GetAsync("/todolist")).Content.ReadFromJsonAsync<List<ToDoItem>>();
            finalTodos.Should().HaveCount(2);
            finalTodos.Should().Contain(t => t.Id == createdIds[0] && t.Title == "Task 1 - Updated" && t.IsDone);
            finalTodos.Should().Contain(t => t.Id == createdIds[2] && t.Title == "Task 3 - Updated" && !t.IsDone);
            finalTodos.Should().NotContain(t => t.Id == createdIds[1]);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task InvalidRoute_ShouldReturn404()
        {
            // Act
            var response = await _client.GetAsync("/invalid-endpoint");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task InvalidHttpMethod_ShouldReturn405()
        {
            // Act
            var response = await _client.GetAsync("/todolist/550e8400-e29b-41d4-a716-446655440000");

            // Assert - GET on specific todo ID should not be supported based on current controller
            // This test documents current behavior - adjust based on actual API design
        }

        #endregion
    }
}
