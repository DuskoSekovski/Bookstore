# Bookstore API

A production-ready, back-end REST API for a Bookstore built with **.NET 10** following **Clean Architecture** principles, which might seem like overkill for a simple CRUD application of this size, but it provides great testability and it's excelent for further scalability.

## Project Overview

This solution provides a fully functional CRUD API for Books (and Authors) protected by OAuth2 Client Credentials, and a paginated Search API protected by OAuth2 Implicit Flow. 

### Architecture
The solution is structured following **Clean Architecture**:
*   **`Bookstore.Domain`**: Core entities (`Book`, `Author`) and repository interfaces. No dependencies on other projects.
*   **`Bookstore.Application`**: Business logic, DTOs, Service interfaces, and FluentValidation validators. 
*   **`Bookstore.Infrastructure`**: Data access implementation (EF Core, SQL Server), migrations, and repository implementations.
*   **`Bookstore.Api`**: The presentation layer (Controllers, Swagger configuration, Exception Handlers).
*   **`tests/Bookstore.UnitTests`**: xUnit-based unit tests for the core business logic using Moq and FluentAssertions.

###  Security (OAuth2)
To ensure the solution is self-contained and easy to run without spinning up an external Identity Provider (like Duende), a custom lightweight OAuth2 token endpoint was implemented in `AuthController`:
*   **Client Credentials Flow (`books.crud`)**: Used for all Create, Read, Update, and Delete operations. Swagger sends the client ID and secret via Basic Auth headers.
*   **Implicit Flow (`books.search`)**: Used exclusively for the Search endpoint. Tokens are returned via URL hash fragment as per the OAuth2 specification.
*   **Authorization Policies**: ASP.NET Core policies (`RequireCrudScope`, `RequireSearchScope`) strictly enforce that a token with only `books.search` permissions cannot access the CRUD endpoints.

---

## Quick Start (Docker)

The entire solution (API + SQL Server) is fully containerized. You only need Docker Desktop installed.

1.  Make sure that Docker Desktop is up and running.
2.  Open a terminal at the root of the solution.
3.  Run the following command: (`docker-compose up -d --build`)
4.  Wait till everything is up and running:
    ✔ Image bookstore-api:latest      Built
    ✔ Network bookstore_default       Created
    ✔ Container bookstore-sql-server  Healthy
    ✔ Container bookstore-api-service Started
5. Open your browser and navigate to Swagger UI: (`http://localhost:8080/swagger`)

## Testing the API (via Swagger)

The database is seeded with 2 Authors and 3 Books on startup so you can immediately test the endpoints.
To test the endpoints, you must authorize in Swagger

1. Click the Authorize button at the top of the page
2. For CRUD Operations, populate **ClientCredentialsAuth (OAuth2, clientCredentials)** form:
   * client_id: admin-client
   * client_secret: super-secret-key
   * Click Authorize

3. For Search, populate **ImplicitAuth (OAuth2, implicit)** form:
    * client_id: swagger-ui
    * Click Authorize

___

## Running Unit Tests

The solution includes unit tests for the BookService, covering happy paths, validation failures, and pagination logic.

Tests can be runned via terminal with the command: (`dotnet test`) or via Visual Studio -> Test tab -> Run All Tests

## Tech Stack

* .NET 10
* Entity Framework Core (Code-First Migrations, SQL Server)
* FluentValidation (for Request DTO validation)
* Microsoft.AspNetCore.OpenApi (Swagger UI with custom OAuth2 Document Transformer)
* xUnit, Moq, FluentAssertions (Unit Testing)
* Docker & Docker Compose (Containerization)
* Global Exception Handling (IExceptionHandler to prevent leaking stack traces)