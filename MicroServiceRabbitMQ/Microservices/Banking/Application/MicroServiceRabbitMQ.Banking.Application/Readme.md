# MicroServiceRabbitMQ.Banking.Application

The **MicroServiceRabbitMQ.Banking.Application** project is the **Application Layer** of the Microservice RabbitMQ solution.

It acts as the bridge between the **API** and the **Domain** layers by coordinating application workflows. It contains **Application Services**, **DTO/Models**, and **Service Interfaces** while delegating business rules to the Domain layer.

> The Application layer orchestrates use cases without containing business logic itself.

It communicates with the Domain layer through repository interfaces and commands, and with the messaging infrastructure through the Event Bus.

---

# Purpose

Instead of controllers directly accessing repositories or creating commands, they communicate with the application service.

```csharp
Controller
      │
      ▼
IAccountService
      │
      ▼
AccountService
      │
      ▼
Domain
```

The Application layer coordinates operations while keeping controllers clean and focused on handling HTTP requests.

---

# Project Structure

```
MicroServiceRabbitMQ.Banking.Application
│
├── Interfaces
│   └── IAccountService.cs
│
├── Models
│   └── AccountTransfer.cs
│
├── Services
│   └── AccountService.cs
│
└── MicroServiceRabbitMQ.Banking.Application.csproj
```

---

# Application Service

The `AccountService` class coordinates account-related operations.

```csharp
public class AccountService : IAccountService
{
    ...
}
```

It receives its dependencies through constructor injection.

```csharp
public AccountService(
    IAccountRepository accountRepository,
    IEventBus bus)
{
    _accountRepository = accountRepository;
    _bus = bus;
}
```

This keeps the service loosely coupled and easy to test.

---

# Service Responsibilities

The `AccountService` is responsible for:

- Retrieving account information
- Creating transfer commands
- Sending commands through the Event Bus
- Coordinating communication between the API and Domain layers

---

# Retrieving Accounts

```csharp
public IEnumerable<Account> GetAccounts()
{
    return _accountRepository.GetAccounts();
}
```

Flow

```
Controller
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
Database
```

The service delegates data access to the repository instead of directly querying the database.

---

# Processing Transfers

```csharp
public void Transfer(AccountTransfer accountTransfer)
{
    var createTransferCommand =
        new CreateTransferCommand(
            accountTransfer.FromAccount,
            accountTransfer.ToAccount,
            accountTransfer.TransferAmount);

    _bus.SendCommand(createTransferCommand);
}
```

Instead of performing the transfer itself, the service creates a command and sends it through the Event Bus.

Flow

```
Controller
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
Event Bus
      │
      ▼
MediatR
      │
      ▼
TransferCommandHandler
```

This follows the CQRS pattern by separating requests from command processing.

---

# Service Interface

The application exposes its functionality through the `IAccountService` interface.

```csharp
public interface IAccountService
{
    IEnumerable<Account> GetAccounts();

    void Transfer(AccountTransfer accountTransfer);
}
```

Using an interface allows the implementation to be replaced or mocked during unit testing.

```
Controller
      │
      ▼
IAccountService
      │
      ▼
AccountService
```

---

# Application Model

The `AccountTransfer` model contains the data required to initiate a transfer.

```csharp
public class AccountTransfer
{
    public int FromAccount { get; set; }
    public int ToAccount { get; set; }
    public decimal TransferAmount { get; set; }
}
```

This model is used to transfer data between the API layer and the Application layer.

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
      ├── IAccountRepository
      │         │
      │         ▼
      │   AccountRepository
      │
      └── IEventBus
                │
                ▼
          RabbitMQ Bus
```

---

# CQRS Flow

```
HTTP Request
      │
      ▼
Controller
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
Database
      │
      ▼
TransferCreatedEvent
      │
      ▼
RabbitMQ
```

---

# Why Use an Application Layer?

Without an Application Layer

```csharp
public IActionResult Transfer(...)
{
    // Repository logic
    // Command creation
    // Event Bus logic
}
```

Problems

- Controllers become large
- Business workflow scattered
- Difficult to maintain
- Hard to test

With an Application Layer

```csharp
public AccountController(
    IAccountService accountService)
{
    _accountService = accountService;
}
```

Benefits

- Thin controllers
- Clear separation of responsibilities
- Better maintainability
- Easier unit testing
- Centralized application workflows

---

# Technologies Used

- .NET
- C#
- Dependency Injection
- MediatR
- RabbitMQ
- CQRS
- Repository Pattern

---

# Key Responsibilities

- Coordinate application workflows
- Expose application services
- Send commands through the Event Bus
- Retrieve data through repositories
- Define application models (DTOs)
- Isolate the API layer from the Domain layer

---

# Summary

The **MicroServiceRabbitMQ.Banking.Application** project serves as the **Application Layer** of the solution. It coordinates interactions between the API and Domain layers through application services, delegates data access to repositories, and sends commands through the Event Bus for processing by MediatR handlers. By keeping orchestration separate from business logic, it promotes a clean, maintainable, and scalable architecture based on **CQRS**, **Dependency Injection**, and the **Repository Pattern**.