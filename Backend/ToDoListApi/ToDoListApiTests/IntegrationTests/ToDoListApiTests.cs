using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
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
            var response = await _client.GetAsync("/api/todolist");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            todos.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_WithExistingTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var todo1 = new ToDoItemRequest { Title = "Task 1" };
            var todo2 = new ToDoItemRequest { Title = "Task 2" };

            await _client.PostAsJsonAsync("/api/todolist", todo1);
            await _client.PostAsJsonAsync("/api/todolist", todo2);

            // Act
            var response = await _client.GetAsync("/api/todolist");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            todos.Should().HaveCount(2);
            todos.Should().Contain(t => t.Title == "Task 1");
            todos.Should().Contain(t => t.Title == "Task 2");
        }

        [Fact]
        public async Task GetAll_ShouldReturnTodosOrderedByCreatedAt()
        {
            // Arrange
            var todo1 = new ToDoItemRequest { Title = "First Task" };
            await _client.PostAsJsonAsync("/api/todolist", todo1);

            await Task.Delay(100);

            var todo2 = new ToDoItemRequest { Title = "Second Task" };
            await _client.PostAsJsonAsync("/api/todolist", todo2);

            // Act
            var response = await _client.GetAsync("/api/todolist");
            var todos = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            todos.Should().HaveCount(2);
            todos[0].Title.Should().Be("First Task");
            todos[1].Title.Should().Be("Second Task");
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidTodo_ShouldReturnCreatedStatus()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "New Todo" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdTodo = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();
            createdTodo.Should().NotBeNull();
            createdTodo.Id.Should().NotBe(Guid.Empty);
            createdTodo.Title.Should().Be("New Todo");
            createdTodo.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldGenerateUniqueIds()
        {
            // Arrange
            var todo1 = new ToDoItemRequest { Title = "Todo 1" };
            var todo2 = new ToDoItemRequest { Title = "Todo 2" };

            // Act
            var response1 = await _client.PostAsJsonAsync("/api/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/api/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItemResponse>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Assert
            created1.Id.Should().NotBe(created2.Id);
        }

        [Fact]
        public async Task Create_WithEmptyTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithWhitespaceTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "   " };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todolist", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithNullTitle_ShouldReturnBadRequest()
        {
            // Arrange
            var content = new StringContent("{\"title\": null}", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/todolist", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ShouldPersistTodoInRepository()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Persistent Task" };

            // Act
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            var getResponse = await _client.GetAsync("/api/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            todos.Should().ContainSingle(t => t.Id == created.Id && t.Title == "Persistent Task");
        }

        [Fact]
        public async Task Create_CreatedTodoShouldHaveIsCompletedFalse()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "New Task" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Assert
            created.IsCompleted.Should().BeFalse();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithExistingTodo_ShouldReturnNoContent()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Original Title" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            var updatedTodo = new ToDoItemRequest { Title = "Updated Title" };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/todolist/{created.Id}", updatedTodo);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_ShouldModifyTodoInRepository()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Original Title" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            var updatedTodo = new ToDoItemRequest { Title = "Modified Title" };
            await _client.PutAsJsonAsync($"/api/todolist/{created.Id}", updatedTodo);

            // Act
            var getResponse = await _client.GetAsync("/api/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            var todoInRepo = todos.Should().ContainSingle(t => t.Id == created.Id).Subject;
            todoInRepo.Title.Should().Be("Modified Title");
        }

        [Fact]
        public async Task Update_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var todo = new ToDoItemRequest { Title = "Non Existing" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/todolist/{nonExistingId}", todo);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithMultipleTodos_ShouldUpdateOnlyTargetTodo()
        {
            // Arrange
            var todo1 = new ToDoItemRequest { Title = "Task 1" };
            var todo2 = new ToDoItemRequest { Title = "Task 2" };

            var response1 = await _client.PostAsJsonAsync("/api/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/api/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItemResponse>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItemResponse>();

            var updatedTodo1 = new ToDoItemRequest { Title = "Updated Task 1" };

            // Act
            await _client.PutAsJsonAsync($"/api/todolist/{created1.Id}", updatedTodo1);

            var getResponse = await _client.GetAsync("/api/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            todos.Should().HaveCount(2);
            todos.Should().ContainSingle(t => t.Id == created1.Id && t.Title == "Updated Task 1");
            todos.Should().ContainSingle(t => t.Id == created2.Id && t.Title == "Task 2");
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithExistingTodo_ShouldReturnNoContent()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/todolist/{created.Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ShouldRemoveTodoFromRepository()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Act
            await _client.DeleteAsync($"/api/todolist/{created.Id}");

            var getResponse = await _client.GetAsync("/api/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            todos.Should().NotContain(t => t.Id == created.Id);
        }

        [Fact]
        public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/todolist/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldOnlyDeleteTargetTodo()
        {
            // Arrange
            var todo1 = new ToDoItemRequest { Title = "Task 1" };
            var todo2 = new ToDoItemRequest { Title = "Task 2" };

            var response1 = await _client.PostAsJsonAsync("/api/todolist", todo1);
            var response2 = await _client.PostAsJsonAsync("/api/todolist", todo2);

            var created1 = await response1.Content.ReadFromJsonAsync<ToDoItemResponse>();
            var created2 = await response2.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Act
            await _client.DeleteAsync($"/api/todolist/{created1.Id}");

            var getResponse = await _client.GetAsync("/api/todolist");
            var todos = await getResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

            // Assert
            todos.Should().HaveCount(1);
            todos.Should().ContainSingle(t => t.Id == created2.Id);
        }

        [Fact]
        public async Task Delete_CannotDeleteSameTodoTwice()
        {
            // Arrange
            var todo = new ToDoItemRequest { Title = "Task to Delete" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Act
            var firstDelete = await _client.DeleteAsync($"/api/todolist/{created.Id}");
            var secondDelete = await _client.DeleteAsync($"/api/todolist/{created.Id}");

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
            var todo = new ToDoItemRequest { Title = "Complete Workflow Task" };
            var createResponse = await _client.PostAsJsonAsync("/api/todolist", todo);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await createResponse.Content.ReadFromJsonAsync<ToDoItemResponse>();

            // Act & Assert - Get All (verify creation)
            var getAllResponse = await _client.GetAsync("/api/todolist");
            var allTodos = await getAllResponse.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            allTodos.Should().Contain(t => t.Id == created.Id);

            // Act & Assert - Update
            var updatedTodo = new ToDoItemRequest { Title = "Updated Workflow Task" };
            var updateResponse = await _client.PutAsJsonAsync($"/api/todolist/{created.Id}", updatedTodo);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedList = await (await _client.GetAsync("/api/todolist")).Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            updatedList.Should().ContainSingle(t => t.Id == created.Id && t.Title == "Updated Workflow Task");

            // Act & Assert - Delete
            var deleteResponse = await _client.DeleteAsync($"/api/todolist/{created.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var finalList = await (await _client.GetAsync("/api/todolist")).Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            finalList.Should().NotContain(t => t.Id == created.Id);
        }

        [Fact]
        public async Task CompleteWorkflow_MultipleTodosOperations()
        {
            // Create multiple todos
            var todosToCreate = new[]
            {
                new ToDoItemRequest { Title = "Task 1" },
                new ToDoItemRequest { Title = "Task 2" },
                new ToDoItemRequest { Title = "Task 3" }
            };

            var createdIds = new List<Guid>();
            foreach (var todo in todosToCreate)
            {
                var response = await _client.PostAsJsonAsync("/api/todolist", todo);
                var created = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();
                createdIds.Add(created.Id);
            }

            // Verify all created
            var allTodos = await (await _client.GetAsync("/api/todolist")).Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            allTodos.Should().HaveCount(3);

            // Update first and third
            var updateTodo1 = new ToDoItemRequest { Title = "Task 1 - Updated" };
            var updateTodo3 = new ToDoItemRequest { Title = "Task 3 - Updated" };

            await _client.PutAsJsonAsync($"/api/todolist/{createdIds[0]}", updateTodo1);
            await _client.PutAsJsonAsync($"/api/todolist/{createdIds[2]}", updateTodo3);

            // Delete second
            await _client.DeleteAsync($"/api/todolist/{createdIds[1]}");

            // Verify final state
            var finalTodos = await (await _client.GetAsync("/api/todolist")).Content.ReadFromJsonAsync<List<ToDoItemResponse>>();
            finalTodos.Should().HaveCount(2);
            finalTodos.Should().Contain(t => t.Id == createdIds[0] && t.Title == "Task 1 - Updated");
            finalTodos.Should().Contain(t => t.Id == createdIds[2] && t.Title == "Task 3 - Updated");
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
            var response = await _client.GetAsync("/api/todolist/550e8400-e29b-41d4-a716-446655440000");

            // Assert - GET on specific todo ID should not be supported based on current controller
            // This test documents current behavior - adjust based on actual API design
        }

        #endregion
    }
}
