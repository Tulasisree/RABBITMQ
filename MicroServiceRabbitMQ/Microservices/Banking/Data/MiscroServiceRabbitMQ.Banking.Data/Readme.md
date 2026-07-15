# MicroServiceRabbitMQ.Banking.Data

The **MicroServiceRabbitMQ.Banking.Data** project is the **Data Access Layer** of the Microservice RabbitMQ solution.

It is responsible for interacting with the database using **Entity Framework Core**. This layer implements the repository interfaces defined in the **Domain** project and provides the application's database context.

> The Data layer defines **how data is stored and retrieved** while keeping database-specific code separate from the business logic.

It follows the **Repository Pattern** and works together with **Entity Framework Core** to provide a clean separation between the Domain and the database.

---

# Purpose

Instead of the Domain layer directly querying the database:

```csharp
var accounts = dbContext.Accounts.ToList();
```

The application communicates through repository interfaces.

```
Application
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

This makes the application easier to maintain, test, and extend.

---

# Project Structure

```
MicroServiceRabbitMQ.Banking.Data
│
├── Context
│   └── BankingDbContext.cs
│
├── Repository
│   └── AccountRepository.cs
│
├── Migrations
│   ├── Initial Migration
│   ├── BankingDB
│   └── BankingDbContextModelSnapshot.cs
│
└── MicroServiceRabbitMQ.Banking.Data.csproj
```

---

# BankingDbContext

The `BankingDbContext` class represents the application's database session.

```csharp
public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
}
```

It inherits from **Entity Framework Core's** `DbContext`.

---

# Responsibilities of DbContext

The `BankingDbContext` is responsible for:

- Managing database connections
- Mapping domain models to database tables
- Tracking entity changes
- Executing SQL queries
- Saving changes to the database

---

# Entity Mapping

```csharp
public DbSet<Account> Accounts { get; set; }
```

This property represents the **Accounts** table.

```
Account Model
        │
        ▼
DbSet<Account>
        │
        ▼
Accounts Table
```

Entity Framework automatically maps the `Account` model to the corresponding database table.

---

# Repository

The repository implements the interface defined in the Domain layer.

```csharp
public class AccountRepository : IAccountRepository
{
    private BankingDbContext _ctx;

    public AccountRepository(BankingDbContext ctx)
    {
        _ctx = ctx;
    }

    public IEnumerable<Account> GetAccounts()
    {
        return _ctx.Accounts;
    }
}
```

---

# Repository Flow

```
Application Service
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

The repository hides all database implementation details from the rest of the application.

---

# Dependency Injection

The repository receives the database context through constructor injection.

```csharp
public AccountRepository(BankingDbContext ctx)
{
    _ctx = ctx;
}
```

ASP.NET Core automatically injects the registered `BankingDbContext`.

Flow

```
Dependency Injection
        │
        ▼
BankingDbContext
        │
        ▼
AccountRepository
```

This eliminates manual object creation and promotes loose coupling.

---

# Entity Framework Migrations

The **Migrations** folder contains Entity Framework Core migrations used to create and update the database schema.

```
Migrations
│
├── Initial Migration
├── BankingDB
└── BankingDbContextModelSnapshot
```

Migrations allow database changes to be version-controlled and applied consistently across environments.

---

# Data Access Flow

```
HTTP Request
      │
      ▼
Controller
      │
      ▼
Application Service
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

# Repository Pattern

Without Repository Pattern

```csharp
var accounts = dbContext.Accounts.ToList();
```

Problems

- Database code scattered throughout the application
- Tight coupling to Entity Framework
- Difficult to unit test
- Harder to replace the data source

With Repository Pattern

```csharp
var accounts = _accountRepository.GetAccounts();
```

Benefits

- Loose coupling
- Easier unit testing
- Cleaner architecture
- Centralized data access
- Better maintainability

---

# Technologies Used

- .NET
- Entity Framework Core
- SQL Server
- Repository Pattern
- Dependency Injection
- LINQ

---

# Key Responsibilities

- Implement repository interfaces
- Configure Entity Framework Core
- Manage database connections
- Map entities to database tables
- Execute database queries
- Maintain database migrations

---

# Summary

The **MicroServiceRabbitMQ.Banking.Data** project is the **Data Access Layer** of the application. It provides the `BankingDbContext` for Entity Framework Core, implements repository interfaces defined in the Domain layer, and manages database operations through the Repository Pattern. By isolating persistence logic from business logic, this layer ensures a clean, maintainable, and scalable architecture.