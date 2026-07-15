"dotnet package add Microsoft.Extensions.DependencyInjection"
---

# MicroServiceRabbitMQ.Infra.IoC

The **MicroServiceRabbitMQ.Infra.IoC** project is the **Dependency Injection (IoC)(Inversion of Control) Layer** of the Microservice RabbitMQ solution.

Its responsibility is to register all application dependencies in a single place, allowing ASP.NET Core's built-in Dependency Injection container to create and manage objects throughout the application.

> Configure which concrete classes should be used when an application requests interfaces.

This project acts as the **composition root**, connecting the **Application**, **Domain**, **Infrastructure**, and **Data** layers without tightly coupling them.

---

# Purpose

Instead of manually creating objects like:

```csharp
var accountService = new AccountService(
    new AccountRepository(new BankingDbContext()));
```

ASP.NET Core automatically creates and injects them by registering all services inside the dependency container.

```
API
 │
 ▼
Dependency Container
 │
 ├── Application Services
 ├── Domain Handlers
 ├── Repositories
 ├── DbContext
 └── RabbitMQ Bus
```

---

# Project Structure

```
MicroServiceRabbitMQ.Infra.IoC
│
├── DependencyContainer.cs
└── MicroServiceRabbitMQ.Infra.IoC.csproj
```

---

# DependencyContainer

The `DependencyContainer` class is responsible for registering every service used by the application.

```csharp
public class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        ...
    }
}
```

It centralizes all dependency registrations so that the API project remains clean and easy to maintain.

---

# Service Registrations

## MediatR

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(DependencyContainer).Assembly);
});
```

Registers **MediatR**, which is responsible for executing commands and requests inside the application.

### Why?

Instead of controllers directly calling business logic, they send commands to MediatR.

```
Controller
      │
      ▼
Send(Command)
      │
      ▼
MediatR
      │
      ▼
Command Handler
```

This keeps controllers lightweight and follows the CQRS pattern.

---

## Event Bus

```csharp
services.AddTransient<IEventBus, RabbitMQBus>();
```

Registers the implementation of the application's event bus.

### Interface

```
IEventBus
```

### Implementation

```
RabbitMQBus
```

Whenever an `IEventBus` is requested, ASP.NET Core creates an instance of `RabbitMQBus`.

### Responsibilities

- Send Commands
- Publish Events
- Subscribe to RabbitMQ Events

---

## Command Handler

```csharp
services.AddTransient<
    IRequestHandler<CreateTransferCommand, bool>,
    TransferCommandHandler>();
```

Registers the command handler responsible for processing `CreateTransferCommand`.

Flow

```
Controller
      │
      ▼
CreateTransferCommand
      │
      ▼
MediatR
      │
      ▼
TransferCommandHandler
```

This keeps business logic outside controllers.

---

## Application Services

```csharp
services.AddTransient<IAccountService, AccountService>();
```

Registers the application service.

### Interface

```
IAccountService
```

### Implementation

```
AccountService
```

The service coordinates business operations between the API layer and the domain layer.

---

## Repository

```csharp
services.AddTransient<IAccountRepository, AccountRepository>();
```

Registers the repository used to access the database.

```
AccountService
        │
        ▼
IAccountRepository
        │
        ▼
AccountRepository
        │
        ▼
SQL Server
```

Using an interface makes it easy to replace the implementation or mock it during unit testing.

---

## Database Context

```csharp
services.AddTransient<BankingDbContext>();
```

Registers Entity Framework Core's `DbContext`.

The `BankingDbContext` is responsible for:

- Managing database connections
- Tracking entity changes
- Executing SQL queries
- Saving changes

---

# Service Lifetime

All services are registered using **Transient**.

```csharp
services.AddTransient<TService, TImplementation>();
```

A new instance is created every time the service is requested.

### Other Service Lifetimes

| Lifetime | Description | Typical Usage |
|----------|-------------|---------------|
| Transient | New instance every request | Services, repositories, handlers |
| Scoped | One instance per HTTP request | DbContext |
| Singleton | One instance for application lifetime | Configuration, caching |

> **Note:** In production applications, `DbContext` is typically registered as **Scoped** using `AddDbContext<TContext>()`, ensuring one context instance per HTTP request.

---

# Dependency Injection Flow

```
HTTP Request
      │
      ▼
Controller
      │
      ▼
IAccountService
      │
      ▼
AccountService
      │
      ▼
IAccountRepository
      │
      ▼
AccountRepository
      │
      ▼
BankingDbContext
      │
      ▼
SQL Server
```

---

# CQRS Flow

```
Controller
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
Repository
      │
      ▼
Database
      │
      ▼
Publish Event
      │
      ▼
RabbitMQ
```

---

# Why Use an IoC Container?

Without Dependency Injection

```csharp
var service = new AccountService(
    new AccountRepository(
        new BankingDbContext()));
```

Problems:

- Tight coupling
- Difficult to test
- Hard to replace implementations
- Manual object creation

With Dependency Injection

```csharp
public AccountController(IAccountService accountService)
{
    _accountService = accountService;
}
```

Benefits:

- Loose coupling
- Better maintainability
- Easier unit testing
- Automatic object creation
- Centralized dependency management

---

# Technologies Used

- .NET
- ASP.NET Core Dependency Injection
- MediatR
- RabbitMQ
- Entity Framework Core
- CQRS
- Repository Pattern

---

# Key Responsibilities

- Register application services
- Register repositories
- Register MediatR handlers
- Register RabbitMQ event bus
- Register Entity Framework DbContext
- Configure dependency injection for the entire application

---

# Summary

The **MicroServiceRabbitMQ.Infra.IoC** project serves as the **composition root** of the application. It wires together all layers of the microservice architecture by registering dependencies with ASP.NET Core's built-in Dependency Injection container. This enables a clean, loosely coupled, and testable architecture while supporting CQRS, MediatR, Entity Framework Core, and RabbitMQ integration.