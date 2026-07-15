# RabbitMQ Microservices using ASP.NET Core

A sample **Event-Driven Microservices** application built using **ASP.NET Core**, **RabbitMQ**, **MediatR**, **CQRS**, **Entity Framework Core**, and **Dependency Injection**.

The solution demonstrates how microservices communicate asynchronously through RabbitMQ while following Clean Architecture and Domain-Driven Design principles.

---

# Solution Structure

```
MicroServiceRabbitMQ
│
├── Domain
│   └── Microservice.Domain.Core
│
├── Infra.Bus
│   └── Microservice.Infrastructure.Bus
│
├── Infra.IoC
│   └── MicroServiceRabbitMQ.Infra.IoC
│
├── Microservices
│   └── Banking
│       ├── Api
│       ├── Application
│       ├── Domain
│       └── Data
│
└── Presentation
```

---

# Architecture

```
                    Client
                       │
                       ▼
                Banking API
                       │
                       ▼
             Application Services
                       │
                  MediatR (CQRS)
                       │
                Command Handler
                       │
            ┌──────────┴──────────┐
            ▼                     ▼
      SQL Server             RabbitMQ Bus
                                   │
                                   ▼
                              Event Queue
                                   │
                                   ▼
                          Other Microservices
```

---

# Projects

## 1. Microservices

The **Microservices** folder contains the business services that make up the application.

Currently, the solution includes the following microservice:

### Banking Microservice

The Banking microservice is responsible for processing account transfers and exposing REST APIs.

It is organized into four layers:

### API

Entry point of the Banking service.

Responsibilities:

- Exposes REST endpoints
- Receives HTTP requests
- Validates incoming requests
- Sends Commands through MediatR
- Returns HTTP responses

---

### Application

Contains the application/business services that coordinate use cases.

Responsibilities:

- Implements application logic
- Defines service interfaces
- Maps request models
- Communicates with the Domain layer

Example:

```
IAccountService
AccountService
AccountTransfer
```

---

### Domain

Contains the core business rules of the Banking service.

Responsibilities:

- Domain Models
- Commands
- Command Handlers
- Domain Events
- Repository Interfaces

Example:

```
Account
TransferCommand
CreateTransferCommand
TransferCreatedEvent
TransferCommandHandler
```

This layer contains the business rules and is independent of infrastructure concerns.

---

### Data

Responsible for data persistence.

Responsibilities:

- Entity Framework Core
- DbContext
- Repositories
- SQL Server
- EF Core Migrations

Example:

```
BankingDbContext
AccountRepository
```

---

# 2. Domain

The **Domain Core** project contains reusable building blocks shared across all microservices.

It provides the abstractions required for implementing CQRS and Event-Driven Architecture.

Contents include:

- Base Message class
- Base Command class
- Base Event class
- IEventBus
- IEventHandler

Example hierarchy:

```
Message
├── Command
└── Event
```

This project contains only contracts and common abstractions, making it reusable across multiple microservices.

---

# 3. Infra.Bus

The Infrastructure Bus project implements the messaging infrastructure.

It provides the concrete implementation of `IEventBus` using RabbitMQ.

Responsibilities include:

- Publishing Events
- Sending Commands
- Registering Event Handlers
- Consuming RabbitMQ messages
- Dispatching messages to handlers

```
Application
      │
      ▼
 IEventBus
      │
      ▼
 RabbitMQBus
      │
      ▼
 RabbitMQ
```

This keeps the messaging implementation isolated from the business logic.

---

# 4. Infra.IoC

The Infrastructure IoC project acts as the application's Composition Root.

It registers all dependencies using ASP.NET Core's Dependency Injection container.

Responsibilities:

- Register Application Services
- Register Repositories
- Register MediatR
- Register RabbitMQ Bus
- Register DbContext
- Configure Dependency Injection

Instead of manually creating objects, ASP.NET Core resolves dependencies automatically through constructor injection.

---

# 5. Presentation

The **Presentation** folder is intended for client-facing applications that consume one or more microservices.

Examples include:

- ASP.NET MVC
- ASP.NET Web API
- Blazor
- Angular
- React
- Mobile Applications
- Desktop Applications

Currently, no presentation projects have been added to this solution.

---

# Technologies Used

- ASP.NET Core
- C#
- .NET 10
- RabbitMQ
- MediatR
- CQRS
- Entity Framework Core
- SQL Server
- Dependency Injection

---

# Design Patterns

This project demonstrates several commonly used architectural patterns:

- Microservices Architecture
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Event-Driven Architecture
- Repository Pattern
- Dependency Injection
- Domain-Driven Design (DDD)
- SOLID Principles

---

# Request Flow

```
Client
   │
   ▼
Banking API
   │
   ▼
Application Service
   │
   ▼
MediatR
   │
   ▼
Command Handler
   │
   ▼
Repository
   │
   ▼
SQL Server
   │
   ▼
Publish Event
   │
   ▼
RabbitMQ
```

---

# Future Enhancements

- Notification Microservice
- Identity Microservice
- Docker Support
- Docker Compose
- Kubernetes Deployment
- API Gateway
- Health Checks
- Distributed Logging
- Dead Letter Queues (DLQ)
- Retry Policies
- Event Versioning
- Distributed Tracing