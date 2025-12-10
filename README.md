# Social-Media Microservices — CQRS, Event Sourcing & Kafka Microservices

This is a distributed microservices system built with **.NET**, implementing **CQRS**, **Event Sourcing**, **Domain-Driven Design (DDD)**, and **Kafka** as an event streaming backbone.  
It consists of two autonomous microservices:

- **Command Microservice** (Write side)
- **Query Microservice** (Read side)

The system manages social media posts, comments, likes, and updates using an event-driven architecture.

---

## 🏗 High-Level Architecture

+-----------------------+        +--------------------------+  
|   Command API (.NET)  | -----> |  Event Store (MongoDB)   |   
+-----------------------+        +--------------------------+  
            |                                |  
            |                                v  
            |                         Kafka Producer  
            |                                |  
            |                             Kafka  
            v                                |  
+-----------------------+        +--------------------------+  
|    Query API (.NET)   | <----- |   Kafka Consumer         |  
+-----------------------+        +--------------------------+  
                                         |  
                                         v  
                               Read DB (SQL Server / PostgreSQL)  
  
---  
  
# 🧩 Command Microservice (Write Model)

The **Command API** is responsible for:

- Handling **state-changing commands**
- Validating intent
- Applying changes through the **Aggregate Root**
- Persisting events into the **Event Store (MongoDB)**
- Publishing events to **Kafka**

### ✨ Core Concepts

#### 🟣 **Aggregate Root**

All domain invariants and state transitions are protected by the `PostAggregate` class.  
It applies events internally using **reflection-based Apply method discovery**:

```csharp
var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
method.Invoke(this, new object[] { @event });
```
This ensures each event calls the correct domain logic without repetitive boilerplate.

#### 📦 **Event Store (MongoDB)**
The EventStore class handles:

- Concurrency checks
- Event versioning
- Persisting events as an append-only stream
- Publishing events to Kafka after saving
```csharp
await _eventProducer.ProduceAsync(topic!, @event);
```
Each event is wrapped into an EventModel that includes:
- Timestamp
- Aggregate ID
- Version
- Event type
- Serialized event data

## 🟠 Dynamic Command Dispatching

The Command API uses a highly flexible dispatcher pattern powered by dependency injection:

```csharp
var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
await handler.HandleAsync(command);
```
## ✅ Benefits

- **Strong decoupling** between controllers and handlers  
- **New commands** can be added without modifying the dispatcher  
- Fully **DI-driven** command routing
## 🔍 Query Microservice (Read Model)

The Query API maintains an optimized read model stored in **SQL Server** or **PostgreSQL**.

### 📌 Flow

1. Kafka consumer receives an event  
2. Event is deserialized using a custom JSON converter  
3. Event handler applies read-model updates  
4. API exposes query endpoints to fetch posts
## 🟡 Polymorphic Deserialization

Events are polymorphic, so a custom JSON converter determines the correct type:

```csharp
public override BaseEvent Read(...)
{
    return typeDiscriminator switch
    {
        nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json),
        // ...
    };
}
```
Each event is mapped explicitly for safety and clarity.

## 🔵 Read Model Projection Handlers

Reflection is used to automatically invoke the correct handler method:

- `On(PostCreatedEvent evt)`
- `On(PostLikedEvent evt)`
- `On(CommentAddedEvent evt)`
- …etc.

These handlers update the relational read database.
## 🔄 Supported Operations

### 🟣 Commands

- `NewPostCommand`
- `EditMessageCommand`
- `LikePostCommand`
- `AddCommentCommand`
- `EditCommentCommand`
- `RemoveCommentCommand`
- `DeletePostCommand`

### 🟤 Queries

- `FindAllPostsQuery`
- `FindPostsByAuthorQuery`
- `FindPostByIdQuery`
- `FindPostsWithCommentsQuery`
- `FindPostsWithLikesQuery`

Query handling uses a dynamic **IQueryDispatcher**.
## 🗄 Databases

### 🟢 Event Store (Write Database)

- **MongoDB**
- Stores all events chronologically  
- Provides full history and replayability

### 🔵 Read Database (Materialized Views)

- **SQL Server** or **PostgreSQL**
- Selected dynamically using environment:

  - `Development` → SQL Server  
  - `Development.PostgreSQL` → PostgreSQL  

- Stores normalized relational tables (**Posts** + **Comments**)
## 📡 Kafka (Event Streaming Backbone)

Kafka propagates domain events:

- **Command service** → Kafka Producer  
- **Query service** → Kafka Consumer  

Topic name is configured via: `KAFKA_TOPIC`
## 🛠 Technologies Used

- **ASP.NET Core**  
- **MongoDB**  
- **Kafka / Confluent.Kafka**  
- **Entity Framework Core**  
- **SQL Server / PostgreSQL**  
- **Domain-Driven Design (DDD)**  
- **CQRS + Event Sourcing**  
- **Reflection-based dispatching**  
- **Microservices architecture**
    
## 🚀 Running the Services

### Requirements

- **MongoDB**  
- **Kafka + Zookeeper**  
- **SQL Server** or **PostgreSQL**

Switch databases by setting the environment variable:

```bash
# Use PostgreSQL
ASPNETCORE_ENVIRONMENT=Development.PostgreSQL

# Use SQL Server
ASPNETCORE_ENVIRONMENT=Development
```

## 📄 Summary

This repository implements a production-ready architecture featuring:

- **Full CQRS pattern**  
- **Event Sourcing**  
- **Kafka-based event streaming**  
- **Dual read/write database model**  
- **DDD patterns**  
- **High-performance read separation**  
- **Dynamic command & query dispatching**
