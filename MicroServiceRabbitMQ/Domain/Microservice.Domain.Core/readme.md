# Microservice.Domain.Core

The **Microservice.Domain.Core** project is the shared **Domain Layer** of the microservices solution.

It contains the common abstractions used by all microservices for implementing **CQRS (Command Query Responsibility Segregation)**, **Event-Driven Architecture**, **MediatR**, and **RabbitMQ** communication.

The project does **not** contain any business logic or infrastructure code. Instead, it provides reusable contracts that allow different microservices to communicate consistently.

---

# Architecture

```
                     Client
                        │
                        ▼
                ASP.NET Core API
                        │
                        ▼
                MediatR (Commands)
                        │
                        ▼
                  Command Handler
                        │
          ┌─────────────┴─────────────┐
          │                           │
          ▼                           ▼
     Database                    Publish Event
                                       │
                                       ▼
                                 RabbitMQ Bus
                                       │
                                       ▼
                              Event Subscribers
```

---

# Project Structure

```
Microservice.Domain.Core
│
├── Bus
│   ├── IEventBus.cs
│   └── IEventHandler.cs
│
├── Commands
│   └── Command.cs
│
├── Events
│   ├── Event.cs
│   └── Message.cs
│
└── Microservice.Domain.Core.csproj
```

---

# Folder Overview

## Bus

Contains interfaces responsible for communication between services.

### IEventBus

Defines the contract for sending commands and publishing events.

```csharp
Task SendCommand<T>(T command);

void Publish<T>(T @event);

void Subscribe<T, TH>();
```

### Responsibilities

- Sends Commands through MediatR
- Publishes Events through RabbitMQ
- Registers Event Handlers

---

### SendCommand()

Used to send commands within the same application using **MediatR**.

Example

```
CreateTransferCommand
        │
        ▼
 SendCommand(command)
        │
        ▼
Command Handler
```

Commands represent **requests to perform an action**.

---

### Publish()

Publishes integration events to RabbitMQ.

Example

```
TransferCompletedEvent
        │
        ▼
 Publish()
        │
        ▼
 RabbitMQ Exchange
        │
        ▼
 Other Microservices
```

Events notify other services that something has already happened.

---

### Subscribe()

Registers an event handler.

Example

```
TransferCompletedEvent
            │
            ▼
TransferCompletedEventHandler
```

Whenever the event arrives from RabbitMQ, the corresponding handler executes.

---

## IEventHandler

Represents the contract for handling events.

```csharp
Task Handle(TEvent @event);
```

Each event should have its own handler.

Example

```
AccountCreatedEvent
        │
        ▼
AccountCreatedEventHandler
```

---

# Commands

Contains the base Command class.

Commands represent **intent**.

Examples

- CreateAccountCommand
- UpdateAccountCommand
- TransferMoneyCommand

A command asks the system to perform an operation.

---

## Command.cs

```csharp
public abstract class Command : Message
```

Every command inherits from Message.

It automatically stores

- Timestamp

```csharp
public DateTime Timestamp { get; protected set; }
```

The timestamp records when the command was created.

Example

```
CreateTransferCommand
        │
        ├── Timestamp
        └── MessageType
```

---

# Events

Contains base classes for integration events.

Events represent something that **already happened**.

Examples

- AccountCreatedEvent
- TransferCompletedEvent
- FundsDepositedEvent

---

## Event.cs

Base class for every event.

```csharp
public abstract class Event
```

Automatically stores

```
Timestamp
```

This helps identify when the event occurred.

---

## Message.cs

Base class for Commands.

```csharp
public abstract class Message : IRequest<bool>
```

This class integrates with **MediatR**.

Every command is also a MediatR request.

It automatically provides

```
MessageType
```

Example

```
CreateTransferCommand

MessageType = "CreateTransferCommand"
```

This is useful for

- Logging
- Auditing
- Debugging
- Event Tracking

---

# CQRS Flow

```
Client
   │
   ▼
Command
   │
   ▼
MediatR
   │
   ▼
Command Handler
   │
   ▼
Database
   │
   ▼
Event
   │
   ▼
RabbitMQ
   │
   ▼
Subscriber
```

---

# Technologies Used

- C#
- .NET
- MediatR
- RabbitMQ
- CQRS
- Event-Driven Architecture

---

# Design Principles

This project follows

- Domain Driven Design (DDD)
- CQRS
- Event-Driven Architecture
- Dependency Inversion Principle
- Interface-based programming

---

# Why this project exists

Instead of every microservice creating its own Command, Event and Event Bus implementations, this shared project provides common abstractions.

Benefits include

- Reusable contracts
- Loose coupling
- Consistent communication
- Easier maintenance
- Cleaner architecture

---

# Example Flow

```
Client
   │
   ▼
CreateTransferCommand
   │
   ▼
SendCommand()
   │
   ▼
TransferCommandHandler
   │
   ▼
Save Data
   │
   ▼
Publish(TransferCreatedEvent)
   │
   ▼
RabbitMQ
   │
   ▼
Banking Service
   │
   ▼
TransferCreatedEventHandler
```

---

# Future Enhancements

- Correlation IDs
- Event Versioning
- Domain Events
- Integration Events
- Retry Policies
- Dead Letter Queue (DLQ)
- Event Sourcing Support

---

# License

This project is intended for educational purposes and demonstrates how to build a reusable Domain layer for a .NET Microservices architecture using CQRS, MediatR, and RabbitMQ.