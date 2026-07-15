# MicroServiceRabbitMQ.Banking.Domain

The **MicroServiceRabbitMQ.Banking.Domain** project is the **Domain Layer** of the Microservice RabbitMQ solution.

It contains the application's core business logic, including **Commands**, **Events**, **Command Handlers**, **Domain Models**, and **Repository Interfaces**. This layer is independent of infrastructure concerns such as databases, APIs, or messaging implementations.

> The Domain layer defines **what the application does**, not **how it is implemented**.

It follows **Domain-Driven Design (DDD)** principles and works closely with **CQRS**, **MediatR**, and **RabbitMQ**.

---

# Purpose

The Domain layer is responsible for defining the business rules and domain operations.

Instead of controllers directly interacting with databases or external services, they communicate with the domain through commands.

```
API
 │
 ▼
Command
 │
 ▼
Command Handler
 │
 ▼
Domain Logic
 │
 ▼
Event
```

This separation keeps business rules centralized and independent.

---

# Project Structure

```
MicroServiceRabbitMQ.Banking.Domain
│
├── CommandHandlers
│   └── TransferCommandHandler.cs
│
├── Commands
│   ├── TransferCommand.cs
│   └── CreateTransferCommand.cs
│
├── Events
│   └── TransferCreatedEvent.cs
│
├── Interfaces
│   └── IAccountRepository.cs
│
├── Models
│   └── Account.cs
│
└── MicroServiceRabbitMQ.Banking.Domain.csproj
```

---

# Domain Components

## Commands

Commands represent requests to perform an action.

Example:

```csharp
public class CreateTransferCommand : TransferCommand
{
    public CreateTransferCommand(int from, int to, decimal amount)
    {
        From = from;
        To = to;
        Amount = amount;
    }
}
```

A command contains only the data required to execute a business operation.

### Flow

```
Controller
      │
      ▼
CreateTransferCommand
```

Commands do not contain business logic.

---

## Command Handler

The command handler processes incoming commands and performs the required business operation.

```csharp
public class TransferCommandHandler :
    IRequestHandler<CreateTransferCommand, bool>
{
    ...
}
```

When a transfer request is received:

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

The handler publishes a domain event after processing the command.

---

# Publishing Events

Inside the handler:

```csharp
_bus.Publish(
    new TransferCreatedEvent(
        request.From,
        request.To,
        request.Amount));
```

Instead of directly communicating with other services, the handler raises an event.

Flow

```
TransferCommandHandler
        │
        ▼
TransferCreatedEvent
        │
        ▼
RabbitMQ
        │
        ▼
Other Microservices
```

This enables loose coupling between services.

---

# Events

Events represent something that has already happened in the system.

Example:

```csharp
public class TransferCreatedEvent : Event
{
    public int From { get; private set; }
    public int To { get; private set; }
    public decimal Amount { get; private set; }
}
```

Unlike commands:

- Commands request an action.
- Events notify that an action has already occurred.

---

# Command vs Event

| Command | Event |
|---------|-------|
| Requests work | Announces completed work |
| Sent to one handler | Can be consumed by many services |
| Imperative | Descriptive |
| Example: CreateTransferCommand | Example: TransferCreatedEvent |

```
CreateTransferCommand
        │
        ▼
TransferCommandHandler
        │
        ▼
TransferCreatedEvent
```

---

# Repository Interface

The domain defines repository contracts without knowing how data is stored.

```csharp
public interface IAccountRepository
{
    IEnumerable<Account> GetAccounts();
}
```

Flow

```
Application Service
        │
        ▼
IAccountRepository
        │
        ▼
Infrastructure Implementation
        │
        ▼
SQL Server
```

This keeps the Domain independent of Entity Framework or any database technology.

---

# Domain Models

The **Account** model represents the banking entity used throughout the application.

Example flow

```
Database
      │
      ▼
AccountRepository
      │
      ▼
Account Model
      │
      ▼
Application
```

Domain models represent business entities rather than database tables.

---

# CQRS Flow

```
HTTP Request
      │
      ▼
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
TransferCreatedEvent
      │
      ▼
RabbitMQ
```

This separation makes the application scalable and easier to maintain.

---

# Dependency Relationships

```
API
 │
 ▼
Application Layer
 │
 ▼
Domain Layer
 │
 ├── Commands
 ├── Command Handlers
 ├── Events
 ├── Models
 └── Repository Interfaces
```

The Domain layer does **not** depend on:

- Database
- Entity Framework Core
- RabbitMQ implementation
- ASP.NET Core

Instead, it depends only on abstractions and domain concepts.

---

# Why Use a Separate Domain Layer?

Without a Domain Layer

```csharp
public IActionResult Transfer(...)
{
    // Business logic
    // Database logic
    // RabbitMQ logic
}
```

Problems

- Business rules mixed with controllers
- Difficult to maintain
- Hard to unit test
- Tight coupling

With a Domain Layer

```csharp
_controller
      │
      ▼
Command
      │
      ▼
Domain Handler
      │
      ▼
Event
```

Benefits

- Centralized business logic
- Loose coupling
- Easier testing
- Better maintainability
- Clear separation of concerns

---

# Technologies Used

- .NET
- C#
- MediatR
- CQRS
- Domain-Driven Design (DDD)
- RabbitMQ Events
- Repository Pattern

---

# Key Responsibilities

- Define domain models
- Define commands
- Handle business commands
- Publish domain events
- Define repository interfaces
- Encapsulate business rules

---

# Summary

The **MicroServiceRabbitMQ.Banking.Domain** project represents the heart of the application. It contains the core business logic, defines commands and events, processes requests through MediatR command handlers, and exposes repository interfaces without depending on infrastructure implementations. By separating business rules from technical concerns, the Domain layer provides a clean, maintainable, and scalable architecture aligned with **CQRS**, **DDD**, and **event-driven microservices**.