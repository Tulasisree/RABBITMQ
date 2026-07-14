"dotnet package add Microsoft.Extensions.DependencyInjection"

# DependencyContainer - Dependency Injection Registration

## Purpose

`DependencyContainer` is responsible for registering application dependencies into the .NET Dependency Injection (DI) container.

It belongs to the **Infrastructure IoC (Inversion of Control)** layer.

The main responsibility is:

> Configure which concrete classes should be used when an application requests interfaces.

---

# Dependency Container

1. Centralizes dependency registration
2. Connects interfaces with implementations
3. Keeps Domain layer independent from Infrastructure
4. Enables Dependency Injection
5. Registers RabbitMQ as the event bus implementation