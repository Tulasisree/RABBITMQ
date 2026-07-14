# RabbitMQBus

## What is RabbitMQBus?

`RabbitMQBus` is the central communication component of our application.

It has two responsibilities:

1. Execute commands inside the current application using MediatR.
2. Publish and receive events through RabbitMQ so that different microservices can communicate.

Think of it as a messenger.

Instead of every part of the application talking directly to RabbitMQ or MediatR, they all communicate through `RabbitMQBus`.

```
                RabbitMQBus
                     |
        +------------+------------+
        |                         |
     Commands                  Events
        |                         |
     MediatR                  RabbitMQ
```

This gives us a single place responsible for sending and receiving messages.

---

## Why do we need it?

Imagine we have an Order Service.

When a customer places an order:

1. The order should be saved to the database.
2. An email should be sent.
3. Inventory should be updated.
4. Billing should start.

If the Order Service directly called Email Service, Inventory Service and Billing Service, the code would become tightly coupled.

Instead, the Order Service simply publishes an event.

```
Order Service
      |
      |
OrderCreatedEvent
      |
      v
RabbitMQ
      |
      +------------+------------+
      |            |            |
      v            v            v
Email      Inventory      Billing
```

Now the Order Service doesn't know who is listening.

It only announces:

> "An order has been created."

Any service interested in that event can subscribe to it.

This is the main idea behind Event-Driven Architecture.

---

## Why are Commands different?

Commands are different from events.

A command tells the system to do something.

Example:

CreateOrderCommand

Only one handler should process it.

```
Controller
     |
     v
RabbitMQBus
     |
     v
MediatR
     |
     v
CreateOrderHandler
```

Commands stay inside the application.

Events travel between applications.

# RabbitMQBus Study Notes

# RabbitMQBus

## What is RabbitMQBus?

`RabbitMQBus` is the communication layer of the application.

It hides the implementation details of MediatR and RabbitMQ behind a single class. The rest of the application only talks to `IEventBus`.

Responsibilities:

- Send Commands using MediatR.
- Publish Events to RabbitMQ.
- Subscribe handlers to events.
- Receive events from RabbitMQ.
- Deserialize messages.
- Invoke the correct event handler using Reflection.

---

# Constructor

```csharp
public RabbitMQBus(IMediator mediator)
```

Stores the MediatR instance and initializes two collections:

- `_handlers` : Maps an event name to the list of handler types.
- `_eventTypes` : Stores all subscribed event CLR types.

---

# SendCommand()

Purpose: Execute a command inside the current application.

Flow:

Controller
→ RabbitMQBus
→ MediatR
→ CommandHandler

Commands stay inside the same process.

---

# Publish()

Purpose: Notify other services that something happened.

Steps:

1. Create a RabbitMQ connection.
2. Create a channel.
3. Use the event class name as the queue name.
4. Serialize the event to JSON.
5. Convert JSON to bytes.
6. Publish to RabbitMQ.

Publisher does not know who is listening.

---

# Subscribe()

Purpose: Register a handler for an event.

Steps:

1. Get event name using `typeof(T).Name`.
2. Save event type.
3. Create a dictionary entry if needed.
4. Prevent duplicate handlers.
5. Store handler type.
6. Start listening to RabbitMQ.

Example:

OrderCreatedEvent
    |
    +-- EmailHandler
    +-- InventoryHandler

---

# StartBasicConsume()

Purpose: Begin listening for messages.

Steps:

1. Connect to RabbitMQ.
2. Declare queue.
3. Create AsyncEventingBasicConsumer.
4. Register callback:

```csharp
consumer.ReceivedAsync += Consumer_Received;
```

5. Start consuming.

Nothing happens until a message arrives.

---

# Consumer_Received()

Called automatically whenever RabbitMQ delivers a message.

Steps:

- Read queue name (RoutingKey).
- Convert bytes to string.
- Call `ProcessEvent()`.

Think of it as the entry point for incoming events.

---

# ProcessEvent()

Purpose: Execute every handler subscribed to an event.

Steps:

1. Find handlers for the event.
2. Create handler object using `Activator.CreateInstance`.
3. Find matching event type.
4. Deserialize JSON into that type.
5. Build `IEventHandler<T>` using reflection.
6. Find the `Handle()` method.
7. Invoke `Handle(event)`.

Flow:

JSON
 ↓
Deserialize
 ↓
OrderCreatedEvent
 ↓
EmailHandler.Handle()
 ↓
InventoryHandler.Handle()

---

# Fields

## _handlers

Dictionary<string,List<Type>>

Maps event names to handler types.

Example:

OrderCreatedEvent
→ EmailHandler
→ InventoryHandler

## _eventTypes

Stores every subscribed event type.

Needed to convert JSON back into the correct CLR object.

---

# Reflection used

- `typeof(T)` → gets Type metadata.
- `.Name` → class name (e.g. OrderCreatedEvent).
- `Activator.CreateInstance()` → create object dynamically.
- `MakeGenericType()` → build generic type at runtime.
- `GetMethod()` → locate Handle().
- `Invoke()` → execute Handle().

---

# Complete Flow

1. Controller sends command.
2. MediatR executes command handler.
3. Command handler publishes eve ent.
4. RabbitMQ stores message.
5. Consumer receives message.
6. Consumer_Received runs.
7. ProcessEvent deserializes JSON.
8. Reflection calls Handle().
9. Event handler performs business logic.

             HTTP Request
                  |
                  |
                  v
             Controller
                  |
                  |
                  v
             RabbitMQBus.SendCommand()
                  |
                  |
                  v
         _mediator.Send()
                  |
                  |
                  v
        MediatR finds handler
                  |
                  |
                  v
        CreateOrderCommandHandler.Handle()
                  |
         Save Order to Database
                  |
                  |
                  v
        _bus.Publish(OrderCreatedEvent)
                  |
                  |
                  v
        RabbitMQBus.Publish()
                  |
                  |
                  v
        RabbitMQ Queue
                  |
                  |
                  v
        consumer.ReceivedAsync
                  |
                  |
                  v
        Consumer_Received()
                  |
                  |
                  v
        ProcessEvent()
                  |
                  |
                  v
            Reflection
                  |
                  |
                  v
        EmailHandler.Handle()

