# MicroServiceRabbitMQ.Banking.Api

The **MicroServiceRabbitMQ.Banking.Api** project is the **Presentation Layer** of the Microservice RabbitMQ solution.

It exposes the application's functionality through **ASP.NET Core Web API** endpoints. The API receives HTTP requests, delegates work to the **Application Layer**, and returns responses to clients without containing business logic.

> The API layer is responsible for handling HTTP communication, routing requests, validating input, and coordinating with the Application layer.

This project serves as the entry point of the application and uses **Dependency Injection**, **ASP.NET Core MVC**, and **Swagger/OpenAPI**.

---

# Purpose

Instead of controllers directly implementing business logic or accessing the database:

```csharp
public IActionResult Transfer(...)
{
    // Business logic
    // Database logic
}
```

The API delegates requests to the Application layer.

```
Client
   │
   ▼
ASP.NET Core API
   │
   ▼
Application Service
   │
   ▼
Domain Layer
   │
   ▼
Data Layer
```

This keeps controllers lightweight and focused on HTTP request handling.

---

# Project Structure

```
MicroServiceRabbitMQ.Banking.Api
│
├── Controllers
│   └── BankingController.cs
│
├── Models
│   └── ErrorViewModel.cs
│
├── Program.cs
└── MicroServiceRabbitMQ.Banking.Api.csproj
```

---

# Program.cs

The `Program.cs` file is the application's startup configuration.

It is responsible for:

- Configuring Dependency Injection
- Registering application services
- Configuring Entity Framework Core
- Configuring RabbitMQ
- Enabling Controllers
- Configuring Swagger/OpenAPI
- Building and running the ASP.NET Core application

Typical flow

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.RegisterServices();

var app = builder.Build();

app.MapControllers();

app.Run();
```

---

# BankingController

The `BankingController` exposes REST API endpoints related to banking operations.

```csharp
public class BankingController : Controller
{
    private readonly IAccountService _accountService;

    public BankingController(
        IAccountService accountService)
    {
        _accountService = accountService;
    }
}
```

The controller receives its dependencies through constructor injection.

---

# Dependency Injection

Instead of creating services manually:

```csharp
var service = new AccountService(...);
```

ASP.NET Core automatically injects them.

```
Dependency Container
        │
        ▼
BankingController
        │
        ▼
IAccountService
```

This keeps controllers loosely coupled and easy to test.

---

# Retrieving Accounts

The controller delegates account retrieval to the Application layer.

Flow

```
HTTP GET
      │
      ▼
BankingController
      │
      ▼
IAccountService
      │
      ▼
AccountRepository
      │
      ▼
Database
```

The controller simply returns the data received from the service.

---

# Creating Transfers

When a transfer request is received:

```
HTTP POST
      │
      ▼
BankingController
      │
      ▼
AccountTransfer
      │
      ▼
AccountService
      │
      ▼
CreateTransferCommand
      │
      ▼
MediatR
      │
      ▼
TransferCommandHandler
      │
      ▼
RabbitMQ
```

The controller does not execute business logic itself; it forwards the request to the Application layer.

---

# API Request Flow

```
HTTP Request
      │
      ▼
Routing
      │
      ▼
Controller
      │
      ▼
Application Service
      │
      ▼
Domain Layer
      │
      ▼
Repository
      │
      ▼
Database
```

---

# Error Model

The `ErrorViewModel` provides information when an error occurs.

```csharp
public class ErrorViewModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId =>
        !string.IsNullOrEmpty(RequestId);
}
```

It helps display or log request-specific error information.

---

# Swagger / OpenAPI

The API project enables **Swagger/OpenAPI**, allowing developers to:

- View available endpoints
- Test APIs from the browser
- Explore request and response models
- Generate API documentation

Typical endpoint

```
/swagger
```

---

# Dependency Injection Flow

```
HTTP Request
      │
      ▼
BankingController
      │
      ▼
IAccountService
      │
      ▼
AccountService
      │
      ▼
Repository
      │
      ▼
Database
```

---

# Complete Architecture Flow

```
Client
      │
      ▼
ASP.NET Core API
      │
      ▼
Application Layer
      │
      ▼
Domain Layer
      │
      ▼
Repository
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
      │
      ▼
RabbitMQ Events
```

---

# Why Use an API Layer?

Without an API Layer

```csharp
Database
    ▲
    │
Client
```

Problems

- No centralized request handling
- No routing
- No validation
- Tight coupling

With an API Layer

```csharp
Client
    │
    ▼
Controller
    │
    ▼
Application Service
```

Benefits

- Thin controllers
- Clear separation of concerns
- Easier maintenance
- Better scalability
- Improved testability
- RESTful endpoints

---

# Technologies Used

- .NET
- ASP.NET Core Web API
- ASP.NET Core MVC
- Dependency Injection
- Swagger / OpenAPI
- Entity Framework Core
- RabbitMQ
- MediatR
- CQRS

---

# Key Responsibilities

- Expose REST API endpoints
- Receive HTTP requests
- Delegate requests to the Application layer
- Return HTTP responses
- Configure Dependency Injection
- Configure Swagger/OpenAPI
- Start and host the application

---

# Summary

The **MicroServiceRabbitMQ.Banking.Api** project serves as the **Presentation Layer** and entry point of the application. It exposes RESTful endpoints through ASP.NET Core, receives client requests, delegates application workflows to the Application layer, and returns responses without containing business logic. By leveraging **Dependency Injection**, **Swagger**, **CQRS**, and a layered architecture, the API remains clean, maintainable, and scalable.