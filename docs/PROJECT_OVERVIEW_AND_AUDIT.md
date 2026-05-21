# WebShopMercantec — project overview (simple explanation) + quick audit

Date: 2026-04-28

## 1) What this project is (in plain words)
This solution is a **web shop system** built as two parts:

- **Backend (server)**: a web server that talks to the database, applies business rules (orders, credits, users), and exposes an API (URLs like `/api/...`).
- **Frontend (client)**: a website that runs in the browser (Blazor WebAssembly). It calls the backend API to show data and perform actions.

To make backend and frontend “speak the same language”, there is a third small project with shared data models.


## 2) Why there are 3 projects in the solution
In `WebShopMercantec.sln` you have:

### A) `WebShopMercantec` (Server)
Folder: `WebShopMercantec/WebShopMercantec/`

**What it does**
- Starts the web server (see `Program.cs`).
- Connects to the database via Entity Framework Core (EF Core).
- Implements API endpoints/controllers (`Controllers/`).
- Contains business logic (`Services/`).
- Contains database models (`Models/`).

**Why it exists**
- The browser cannot safely access your database directly.
- All important rules (permissions, credits, order status rules) must live on the server.

### B) `WebShopMercantec.Client` (Browser UI)
Folder: `WebShopMercantec/WebShopMercantec.Client/`

**What it does**
- Runs in the browser (Blazor WebAssembly).
- Shows pages/components (`Components/Pages/*.razor`).
- Sends HTTP requests to the server API using `HttpClient`.
- Handles login state (JWT token in browser storage) in `Auth/`.

**Why it exists**
- This is the “website” the user sees.

### C) `WebShopMercantec.Shared` (Shared DTOs)
Folder: `WebShopMercantec.Shared/`

**What it does**
- Stores shared **DTOs** (Data Transfer Objects) in `DTOs/`.

**What is a DTO?**
A DTO is a *simple data container* used to send data over the network.
Example: `OrderDto`, `UserDto`.

**Why it exists**
- Backend and frontend use the same DTO classes, so they agree on the shape of JSON data.


## 3) Big picture: how a request works
1. User clicks a button in the browser (Client).
2. Client calls backend API, e.g. `POST /api/orders`.
3. Backend controller receives the request (`Controllers/OrdersController.cs`).
4. Backend service applies business rules (`Services/OrderService.cs`).
5. Repository/DbContext reads/writes to database (`Repositories/*`, `Models/SnipeItContext`).
6. Backend returns DTO JSON.
7. Client shows result.


## 4) Where things are (map of the repository)
Top level (root):
- `WebShopMercantec.sln` — the solution file that opens all projects together.
- `Dockerfile`, `docker-compose.yml` — container setup (run the app in Docker).
- `migrations/*.sql` — SQL scripts to create/update database tables.
- `docs/` — documentation files.
- `notes-local/` — internal notes/checklists (not runtime code).

### Server project (`WebShopMercantec/WebShopMercantec/`)
- `Program.cs` — **startup**: configures DB, authentication, services, controllers, static files.
- `Controllers/` — API endpoints (URLs).
- `Services/` — business logic.
- `Repositories/` — database access code.
- `Models/` — EF Core entities + `SnipeItContext` (database context).
- `Middleware/` — cross-cutting logic (e.g., error handling).
- `Configuration/` — settings classes and Serilog configuration.

### Client project (`WebShopMercantec/WebShopMercantec.Client/`)
- `Program.cs` — startup for browser app, registers `HttpClient`, auth.
- `Components/Pages/` — pages (routes like `/login`, `/admin/orders`, etc.).
- `Components/Layout/` — layout components (header/nav/footer).
- `Auth/` — JWT token storage + authentication state.
- `wwwroot/` — static assets (CSS, images).

### Shared project (`WebShopMercantec.Shared/`)
- `DTOs/` — shared request/response models.


## 5) About the navbar: why you see “extra” navigation
You currently have **two different navigation implementations** in the client:

### 5.1 `MainLayout.razor` — the *actual* navbar used by the app
File: `WebShopMercantec.Client/Components/Layout/MainLayout.razor`

This layout contains a custom `<nav class="navbar"> ...` menu.

The router uses this layout by default:
- File: `WebShopMercantec.Client/Components/Routes.razor`
- Line: `DefaultLayout="typeof(Layout.MainLayout)"`

So, **MainLayout is what the user really sees**.

### 5.2 `NavMenu.razor` — a leftover template sidebar menu
File: `WebShopMercantec.Client/Components/Layout/NavMenu.razor`

This component is the standard Blazor template “sidebar navigation”.
But because your app uses **MainLayout**, and MainLayout does not include NavMenu,
**NavMenu can appear “not working / unused”**.

**Answer to your question:**
- Yes, NavMenu is very likely **not used** in the running UI (unless some page/layout includes it).
- It’s not “harmful”, but it can confuse people because editing it may not affect the visible UI.

If someone asks “where is the menu?”, the correct answer is:
- “The visible menu is in `MainLayout.razor` (top navbar). `NavMenu.razor` is a template leftover and is currently not part of the UI.”


## 6) Quick audit: what looks “not ideal” (short, to the point)
The solution builds successfully, but it produces **many warnings** (code quality hints).
Warnings don’t stop the app, but they are signs of things to improve.

### 6.1 Mixed language in code/comments/UI
- Example: `MainLayout.razor` had Russian comments.
- Example: some UI strings are Russian (admin page pager text previously).

**Why it’s not ideal:** hard to maintain in an international team.

### 6.2 Potential null reference in UI
Build warning:
- `UserProfile.razor`: `CS8602 Dereference of a possibly null reference`

**Why it matters:** can crash some pages at runtime.

### 6.3 Culture-dependent string/Date parsing
Many warnings like:
- `ToUpper()/ToLower()` without specifying culture.
- `DateTime.Parse()` without specifying invariant culture.

**Why it matters:** behavior can differ on different OS language settings.

### 6.4 Many “public fields” warnings (CA1051)
Warnings like:
- `HomeBase.cs`: “Do not declare visible instance fields”

**Why it matters:** public fields are harder to evolve safely than properties.

### 6.5 Performance/logging best practices (not critical)
Warnings like:
- “Prefer LoggerMessage delegates”

**Why it matters:** only relevant under heavy load; not a functional bug.

### 6.6 Authentication/role naming consistency
- Roles are embedded into JWT in server `TokenService.cs`.
- Client uses `<AuthorizeView Roles="...">` checks.

**Risk:** if role names differ (e.g. `Super Admin` vs `SuperAdmin`), admin UI links can disappear even for admins.

### 6.7 Anti-forgery + JWT API combination (needs clarity)
Server has:
- `AddAntiforgery()` and `UseAntiforgery()`
- JWT authentication for API

**Risk:** antiforgery usually protects cookie-based forms; for pure JWT APIs it can cause “403” if not configured carefully.
(Your app might be fine, but it’s a common source of surprises.)


## 7) If someone asks you “where is X?” (quick answers)
- **Where are API endpoints?** `WebShopMercantec/Controllers/`
- **Where is business logic?** `WebShopMercantec/Services/`
- **Where is database access?** `WebShopMercantec/Repositories/` and `WebShopMercantec/Models/SnipeItContext`
- **Where are UI pages?** `WebShopMercantec.Client/Components/Pages/`
- **Where is the visible site menu?** `WebShopMercantec.Client/Components/Layout/MainLayout.razor`
- **Where is login/JWT handling (client)?** `WebShopMercantec.Client/Auth/`
- **Where are shared request/response models?** `WebShopMercantec.Shared/DTOs/`


## 8) One-sentence description of `Program.cs` (server)
File: `WebShopMercantec/WebShopMercantec/Program.cs`

It wires everything together:
- database connection,
- repositories + services,
- JWT authentication,
- CORS,
- middleware,
- serving the Blazor client files,
- routing `/api/...` to controllers.

---

If you want, I can also generate a **short “for managers”** version (5–7 bullets) explaining the architecture without code terms.
