# ToDoList — Frontend (Angular)

This is the frontend for the ToDoList example app (Angular 20).

**Quick Start**

- **Install deps:**

```powershell
cd C:\Workspace\ToDoList\Frontend
npm install
```

- **Run the dev server:**

```powershell
npm start
```

Open http://localhost:4200 in your browser. The app uses a backend API at `http://localhost:5168/api/todolist` by default — see Environment below.

**Environment configuration**

- The API base url is stored in `src/environments/environment.ts` (and `environment.prod.ts`). Change `apiBaseUrl` there for local testing or production deployments.
- If you use `ng build --configuration production`, Angular's file replacements (configured in `angular.json`) should replace `environment.ts` with `environment.prod.ts`.

**Running tests**

- Unit tests (Karma + Jasmine):

```powershell
npm test
```

- Added tests:
  - `src/app/todolist/todolist.service.spec.ts` — uses `HttpClientTestingModule` to verify HTTP calls (GET/POST/PUT/DELETE).
  - `src/app/todolist/todolist.component.spec.ts` — uses a spy `TodoListService` to test component behaviors (load, add, toggle, delete).

If Karma opens a browser, test results will appear there; the command will also report failures in the terminal.

**Theme and UI notes**

- The app includes a light/dark theme implemented with CSS variables in `src/styles.scss`. The current theme is stored in `localStorage` and applied via a `data-theme` attribute on the document root.
- There is a theme toggle button in the app header.

**Project structure (relevant files)**

- `src/app/todolist/` — feature module (lazy-loaded), component, service, styles and tests.
- `src/environments/*` — environment configuration (apiBaseUrl, production flag).

**HTTP provider**

- The project uses the modern `provideHttpClient()` provider (configured in `src/app/app-module.ts`) instead of the older `HttpClientModule` import.

**Tips**

- Ensure the backend is running at the configured `apiBaseUrl` before using the UI. The backend endpoints expected are:
  - GET /api/todolist
  - POST /api/todolist
  - PUT /api/todolist/{id}
  - DELETE /api/todolist/{id}
- To change the API endpoint without editing the source files, update `src/environments/environment.ts`.
