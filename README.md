# ToDoList Application

A full-stack Todo List application built with **ASP.NET Core 10.0** (Backend) and **Angular 20** (Frontend).

## Project Overview

This project consists of two main components:

- **Backend**: A RESTful API built with ASP.NET Core following Clean Architecture principles
- **Frontend**: A responsive Angular web application with light/dark theme support

## Prerequisites

Before running this project, ensure you have the following installed:

- **.NET SDK 10.0** or later — [Download](https://dotnet.microsoft.com/download)
- **Node.js 20** (includes npm) — [Download](https://nodejs.org/)
- A code editor (Visual Studio Code, Visual Studio 2022, etc.)

## Project Structure

```
ToDoList/
├── Backend/                    # ASP.NET Core API
│   ├── ToDoListApi/           # Main API project
│   ├── ToDoListApi.Domain/    # Domain layer (business logic)
│   ├── ToDoListApi.Repository/ # Infrastructure layer (data access)
│   └── ToDoListApiTests/      # Test projects
│
└── Frontend/                   # Angular application
    ├── src/                    # Source code
    ├── public/                 # Static assets
    └── package.json            # Dependencies
```

## Quick Start — Running Both Together

### Step 1: Start the Backend

Open a **PowerShell terminal** and run:

```powershell
cd Backend/ToDoListApi
dotnet restore
dotnet build
dotnet run
```

The API will be available at:

- **HTTP**: `http://localhost:5168`
- **HTTPS**: `https://localhost:7006`

### Step 2: Start the Frontend (in a new terminal)

Open a **new PowerShell terminal** and run:

```powershell
cd Frontend
npm install
npm start
```

The Angular app will open at `http://localhost:4200` in your browser.

## How They Communicate

- The **Frontend** calls the **Backend API** at `http://localhost:5168` (or `https://localhost:7006`)
- API endpoints the Frontend uses:
  - `GET /api/todolist` — Get all todos
  - `GET /api/todolist/{id}` — Get a specific todo
  - `POST /api/todolist` — Create a new todo
  - `PUT /api/todolist/{id}` — Update a todo
  - `DELETE /api/todolist/{id}` — Delete a todo

## Backend Details

For detailed information about the Backend API, see [Backend/README.md](./Backend/README.md)

**Key features:**

- Clean Architecture with separated layers (API, Domain, Repository)
- In-memory data storage (no database required for local testing)
- Comprehensive unit and integration tests
- CORS enabled for local development (localhost:4200)
- Global exception handling

**Common Backend Commands:**

```powershell
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run

# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Frontend Details

For detailed information about the Frontend, see [Frontend/README.md](./Frontend/README.md)

**Key features:**

- Modern Angular 20 with standalone components
- Light/dark theme support
- HTTP client for API communication
- Unit tests with Jasmine and Karma
- Lazy-loaded feature modules

**Common Frontend Commands:**

```powershell
# Install dependencies
npm install

# Run dev server (http://localhost:4200)
npm start

# Build for production
npm run build

# Run unit tests
npm test

# Run tests with watch mode
npm test -- --watch
```

## Configuration

### Backend Configuration

- **Port**: Configured in `Backend/ToDoListApi/Properties/launchSettings.json`
- **CORS**: Configured in `Backend/ToDoListApi/Program.cs` (allows localhost:4200)
- **Logging**: Configured in `Backend/ToDoListApi/appsettings.json`

### Frontend Configuration

- **API Base URL**: Configured in `Frontend/src/environments/environment.ts`
  - Update `apiBaseUrl` if your backend runs on a different port
- **Port**: Configured in `Frontend/angular.json` (default: 4200)

## Testing

### Backend Tests

```powershell
cd Backend
dotnet test
```

Tests include:

- Unit tests for services and mappers
- Integration tests for API endpoints

### Frontend Tests

```powershell
cd Frontend
npm test
```

Tests include:

- Component tests
- Service tests with HTTP mocking
- Theme toggle functionality tests

## Troubleshooting

### "Cannot connect to backend"

- Ensure the backend is running (`dotnet run` in `Backend/ToDoListApi/`)
- Check that the API is accessible at `http://localhost:5168`
- Verify the frontend's `apiBaseUrl` in `src/environments/environment.ts`

### "Port 5168 is already in use"

- Change the port in `Backend/ToDoListApi/Properties/launchSettings.json`
- Update the frontend's `apiBaseUrl` to match the new port

### "Port 4200 is already in use"

- Kill the process using the port or specify a different port:
  ```powershell
  ng serve --port 4300
  ```

### CORS errors in the browser console

- Ensure the backend is running in development mode
- Verify CORS policy includes `http://localhost:4200` in `Program.cs`

## Architecture Highlights

### Backend (Clean Architecture)

- **API Layer**: Controllers, request/response models, mappers
- **Domain Layer**: Business logic, service interfaces, domain models
- **Repository Layer**: Data access implementations

### Frontend (Component-Based)

- **Standalone Components**: Modern Angular approach without NgModule
- **Services**: HTTP communication with the backend
- **Feature Modules**: Lazy-loaded for performance

## Technologies Used

### Backend

- ASP.NET Core 10.0
- C# 13
- xUnit for testing
- Moq for mocking
- FluentAssertions for test assertions

### Frontend

- Angular 20
- TypeScript 5.9.2
- RxJS for reactive programming
- Karma and Jasmine for testing
- CSS with CSS variables for theming

## Notes

- Data is stored **in-memory** and will be lost when the backend restarts
- For production, integrate a real database (SQL Server, PostgreSQL, etc.)
- HTTPS is enabled in development mode (requires certificate trust)

## Next Steps

1. Start both the backend and frontend as described above
2. Open `http://localhost:4200` in your browser
3. Create, read, update, and delete todos
4. Toggle between light and dark themes
5. Check the browser console and terminal for any errors

## Support

For issues or questions, refer to the individual README files:

- Backend issues: See [Backend/README.md](./Backend/README.md)
- Frontend issues: See [Frontend/README.md](./Frontend/README.md)
