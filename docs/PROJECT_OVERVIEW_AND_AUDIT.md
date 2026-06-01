# WebShopMercantec: Technical Documentation

## 1. Introduction
This document serves as the primary technical reference for the WebShopMercantec project. It is intended for software engineers, DevOps professionals, and system administrators responsible for developing, maintaining, and deploying the application.

## 2. System Architecture
WebShopMercantec is a distributed web application built on the .NET 9.0 stack. The solution is structured to cleanly separate concerns across three main projects:

### 2.1 Backend API (`WebShopMercantec`)
- **Framework:** ASP.NET Core 9.0
- **Role:** Serves as the central backend API. It encapsulates business logic, authorization, and data access.
- **Data Access:** Entity Framework Core (EF Core) interacting with an existing MySQL database (Snipe-IT schema).
- **Custom Tables:** The Snipe-IT schema is extended with custom WebShop tables (e.g., `webshop_user_credits`, `webshop_credit_transactions`, `webshop_orders`, `webshop_refresh_tokens`).
  > ⚠️ **Important Governance Note:** Currently, these custom extensions are deployed only on the experimental VM where the development took place. The final architectural decision regarding whether these tables should remain integrated within the primary Snipe-IT database or be moved to a separate microservice/database must be discussed and approved by upper management prior to production deployment.
- **Critical EF Core Mapping Note:** The physical hardware table in the database is named `asset` (singular). EF Core must be explicitly configured using `.ToTable("asset")` on the model entity to prevent HTTP 500 mapping errors.

### 2.2 Frontend Client (`WebShopMercantec.Client`)
- **Framework:** Blazor WebAssembly (WASM)
- **Role:** A client-side Single Page Application (SPA) running entirely in the user's browser. It communicates with the backend via RESTful HTTP calls.
- **Authentication:** Token-based authentication using JSON Web Tokens (JWT). The tokens are negotiated with the backend and stored locally in the browser to maintain session state.

### 2.3 Shared Library (`WebShopMercantec.Shared`)
- **Role:** Defines standard Data Transfer Objects (DTOs) used by both the client and server. This ensures strict contract adherence and type safety during API communication, eliminating duplicated models.

## 3. DevOps & Deployment Guide
The application is fully containerized, enabling consistent runtimes and simplified deployments across staging and production environments.

### 3.1 Docker Implementation
The application utilizes an optimized, multi-stage `Dockerfile`:
- **Stage 1 (Build):** Leverages the full `.NET 9.0 SDK` image. It isolates the dependency restoration step (caching NuGet layers) before copying the rest of the source tree, thereby speeding up subsequent builds. The final artifact is generated using `dotnet publish`.
- **Stage 2 (Runtime):** Utilizes the stripped-down `.NET 9.0 ASP.NET Core` runtime image. For security and compliance, the application drops root privileges and executes as the least-privileged `appuser`. The container is configured to listen on port `8080`.

### 3.2 Docker Compose & Network Topology
The deployment topology is defined in `docker-compose.yml`:
- **Service Name:** `webshop`
- **Port Mapping:** Exposes internal port `8080` to host port `8060`.
- **Network Integration:** The container explicitly attaches to an existing external Docker network named `snipeit_default`. This allows the application to directly communicate with the Snipe-IT database container via internal DNS resolution (`snipeit-db-1`).

### 3.3 Required Environment Variables
For the container and application to initialize successfully, the following infrastructure secrets must be injected into the environment:
- `DB_PASSWORD`: The production password for the `snipeit` MySQL service user.
- `JWT_SECRET_KEY`: A cryptographically secure, high-entropy key used to sign and validate JSON Web Tokens.

### 3.4 Container Lifecycle Commands
The deployment is managed via standard Docker Compose workflows:
- **Build and Start (Detached):**
  ```bash
  docker-compose up -d --build
  ```
- **Inspect Real-time Logs:**
  ```bash
  docker-compose logs -f webshop
  ```
- **Halt Services:**
  ```bash
  docker-compose down
  ```

## 4. Developer Setup
To build, test, and run the application locally on a workstation:

1. **Prerequisites:** Ensure the .NET 9.0 SDK is installed.
2. **Network Access:** Local development requires an active Twingate connection and an SSH tunnel to the staging/production database server.
3. **Database Tunnel:** Establish an SSH tunnel to route local port 3307 to the remote Snipe-IT database container:
   ```bash
   ssh -L 3307:127.0.0.1:3306 -N root@10.132.129.101
   ```
4. **Configuration:** Override the settings in `WebShopMercantec/appsettings.Development.json` with the appropriate local database connection string (`ConnectionStrings__DefaultConnection`), pointing to `localhost:3307`.
5. **Execution:** Use the `dotnet` CLI to build and run the backend project (which will also serve the Blazor client):
   ```bash
   dotnet build
   dotnet run --project WebShopMercantec/WebShopMercantec/WebShopMercantec.csproj
   ```

## 5. Core Business Logic
### 5.1 Catalog Visibility (The Triple-Filter)
Items are only visible in the WebShop if they pass a strict triple-filter validation:
1. **Model Requirement:** The underlying Model must be marked as `requestable`.
2. **Asset Requirement:** The specific Asset must be `requestable`, have no current assignee (`assigned_to` is null/0), and must not be marked as `deleted`.
3. **Status Label Requirement:** The Status Label attached to the asset must be `deployable`.

## 6. Security Context
- **Token Claims:** Access roles are embedded within the JWT generated by the backend `TokenService`. Mismatched configurations here may lead to unauthorized routing in the Blazor client.
- **Anti-forgery:** The API incorporates anti-forgery configurations. Ensure that token authorization flows do not blindly trip CSRF protections unless explicitly required.
