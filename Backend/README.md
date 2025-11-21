# ToDo List API

A simple RESTful API for managing todo items, built with ASP.NET Core 10.0.

## Features

- Create, read, update, and delete todo items
- Get all todo items or a specific item by ID
- Model validation
- Comprehensive unit and integration tests
- Structured logging
- Clean architecture with separation of concerns

## Architecture

The project follows Clean Architecture principles with the following layers:

- **ToDoListApi**: API layer (Controllers, Models, Mappers, Validation)
- **ToDoListApi.Domain**: Domain layer (Business logic, Interfaces, Domain models)
- **ToDoListApi.Repository**: Infrastructure layer (In-memory repository implementation)

**Note:** The repository project folder is named `ToDoListApi.Repository` but uses the namespace `ToDoListApi.Infrastructure`.

## Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022, VS Code, or any IDE that supports .NET

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd Backend/ToDoListApi
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Build the solution

```bash
dotnet build
```

### 4. Run the application

```bash
cd ToDoListApi
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5168`
- HTTPS: `https://localhost:7006`

## API Endpoints

### Get All Todo Items
```
GET /api/todolist
```

### Get Todo Item by ID
```
GET /api/todolist/{id}
```

### Create Todo Item
```
POST /api/todolist
Content-Type: application/json

{
  "title": "My new task",
  "isDone": false
}
```

**Note:** `isDone` is optional and defaults to `false`. Title is required and will be trimmed automatically.

### Update Todo Item
```
PUT /api/todolist/{id}
Content-Type: application/json

{
  "title": "Updated task",
  "isDone": true
}
```

**Note:** Both `title` and `isDone` are required in the request body. Title will be trimmed automatically.

### Delete Todo Item
```
DELETE /api/todolist/{id}
```

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run with coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Project Structure

```
ToDoListApi/
├── ToDoListApi/                    # API Layer
│   ├── Controllers/                # API Controllers
│   ├── Models/                     # Request/Response DTOs
│   ├── Mappers/                    # Domain/API mapping
│   ├── Validation/                 # Custom validation attributes
│   └── Program.cs                  # Application entry point
├── ToDoListApi.Domain/             # Domain Layer
│   ├── Interfaces/                 # Service and Repository interfaces
│   ├── Models/                     # Domain models
│   └── Services/                   # Business logic
├── ToDoListApi.Repository/         # Infrastructure Layer
│   └── InMemoryToDoRepository.cs   # In-memory repository implementation
└── ToDoListApiTests/               # Test Project
    ├── UnitTests/                  # Unit tests
    └── IntegrationTests/           # Integration tests
```

## Technologies Used

- ASP.NET Core 10.0
- xUnit for testing
- Moq for mocking
- FluentAssertions for test assertions

## Notes

- The current implementation uses an in-memory repository, so data is not persisted between application restarts
- CORS is enabled for local development (localhost:4200 only)
- The API follows RESTful conventions
- All endpoints include proper HTTP status codes and error handling
- Input validation: Titles are automatically trimmed and validated (required, max 500 characters)
- Results are sorted by creation date (oldest first)

## Future Enhancements

- Database persistence (Entity Framework Core)
- Authentication and authorization
- Pagination for Get All endpoint
- Filtering and sorting
- Soft delete functionality
- Audit logging

