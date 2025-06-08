# Modulith Template

A template for creating modular monolith applications with PostgreSQL, vector search, and graph capabilities.

## Features

- Modular monolith architecture
- PostgreSQL with pgvector for vector search
- Apache AGE for graph operations
- Layered architecture (Domain, Application, Infrastructure, Api)
- Platform services for shared functionality

## Docker & Docker Compose

This template includes Docker and Docker Compose support for local development and deployment.

### Prerequisites
- [Docker](https://www.docker.com/get-started) installed
- [Docker Compose](https://docs.docker.com/compose/) (usually included with Docker Desktop)

### Usage

#### 1. Build and Run All Services

```sh
docker-compose up --build
```

This will start:
- PostgreSQL (with pgvector and Apache AGE)
- Redis
- RabbitMQ (with management UI)
- Seq (logging)
- The main API (Modulith.Web by default)

#### 2. Stopping and Cleaning Up

To stop the containers:
```sh
docker-compose down
```

To remove all data volumes (including the database):
```sh
docker-compose down -v
```

#### 3. Overriding Environment Variables

You can override environment variables (such as connection strings) in the `docker-compose.yml` file or by using a `.env` file.

#### 4. Accessing Services
- **Main API**: http://localhost:8080
- **PostgreSQL**: localhost:5432 (user: postgres, password: postgres)
- **Redis**: localhost:6379
- **RabbitMQ UI**: http://localhost:15672 (user: guest, password: guest)
- **Seq (logs)**: http://localhost:5341

#### 5. Building Individual Services

To build and run a specific service (e.g., a module API):
```sh
docker build -f NewModule/Modulith.NewModule.Api/Dockerfile -t newmoduleapi .
docker run -p 5000:80 newmoduleapi
```

#### 6. Logs

To view logs for all services:
```sh
docker-compose logs -f
```

---

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 15 or later
- pgvector extension
- Apache AGE extension

## PostgreSQL Setup

### 1. Install PostgreSQL Extensions

```sql
-- Install pgvector
CREATE EXTENSION vector;

-- Install Apache AGE
CREATE EXTENSION age;
```

### 2. Configure Connection String

In your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=your_database;Username=your_username;Password=your_password"
  }
}
```

## Database Migrations

### 1. Add Migration

```bash
# Add a new migration
dotnet ef migrations add InitialCreate --project YourModule.Infrastructure --startup-project YourModule.Api
```

### 2. Configure Migration

In your migration's `Up` method:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Ensure extensions exist
    migrationBuilder.EnsureExtensionsExist();

    // Your migration code here
    migrationBuilder.CreateTable(
        name: "Documents",
        columns: table => new
        {
            Id = table.Column<int>(type: "integer", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Content = table.Column<string>(type: "text", nullable: false),
            Embedding = table.Column<float[]>(type: "vector(1536)", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Documents", x => x.Id);
        });
}
```

### 3. Apply Migration

```bash
# Apply migrations
dotnet ef database update --project YourModule.Infrastructure --startup-project YourModule.Api
```

### 4. Remove Migration

```bash
# Remove the last migration
dotnet ef migrations remove --project YourModule.Infrastructure --startup-project YourModule.Api
```

### 5. Migration Best Practices

1. **Extension Management**:
   - Always call `EnsureExtensionsExist()` at the start of migrations
   - Use `suppressTransaction: true` for extension operations
   - Handle extension dependencies in the correct order

2. **Vector Columns**:
   - Specify vector dimensions in the column type
   - Use appropriate indexing for vector columns
   - Consider using HNSW index for large datasets

3. **Graph Operations**:
   - Create necessary indexes for graph traversal
   - Handle graph schema changes carefully
   - Consider using separate migrations for graph schema changes

4. **Data Seeding**:
   - Use separate migrations for data seeding
   - Handle extension-dependent data carefully
   - Consider using SQL scripts for complex data operations

## Vector Operations

The template includes support for vector operations using pgvector. Here's how to use it:

### 1. Define Vector Properties

```csharp
public class Document
{
    public int Id { get; set; }
    public string Content { get; set; }
    public float[] Embedding { get; set; }
}
```

### 2. Configure Entity

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Document>(entity =>
    {
        entity.Property(e => e.Embedding)
            .HasColumnType("vector(1536)"); // Specify vector dimensions
    });
}
```

### 3. Use Vector Operations

```csharp
// Find similar documents
var similarDocs = await dbContext.Documents
    .OrderByCosineDistance(x => x.Embedding, queryVector)
    .Take(10)
    .ToListAsync();

// Find documents within distance
var nearbyDocs = await dbContext.Documents
    .Where(x => EF.Functions.EuclideanDistance(x.Embedding, queryVector) < 0.5)
    .ToListAsync();
```

## Graph Operations

The template includes support for graph operations using Apache AGE. Here's how to use it:

### 1. Define Graph Entities

```csharp
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Relationship
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int FromPersonId { get; set; }
    public int ToPersonId { get; set; }
}
```

### 2. Use Graph Operations

```csharp
// Find paths between nodes
var paths = await dbContext.ExecuteGraphQueryAsync<Path>(
    "MATCH p=(a:Person)-[:FRIEND*]->(b:Person) WHERE a.name = $name RETURN p",
    new { name = "John" });

// Create relationships
await dbContext.ExecuteGraphQueryAsync(
    "MATCH (a:Person), (b:Person) WHERE a.name = $name1 AND b.name = $name2 CREATE (a)-[:FRIEND]->(b)",
    new { name1 = "John", name2 = "Jane" });
```

## Module Structure

Each module follows a layered architecture:

```
ModuleName/
├── Domain/           # Entities and domain logic
├── Application/      # Business logic and use cases
├── Infrastructure/   # Data access and external services
└── Api/             # API endpoints
```

### Platform Services

The platform layer provides shared services:

```
Platform/
├── Domain/          # Shared entities
├── Application/     # Shared services
└── Infrastructure/  # Shared infrastructure
```

## Creating a New Module

1. Create a new module:
```bash
dotnet new modulith --add basic-module --with-name YourModule
```

2. Configure the module's DbContext:
```csharp
public class YourModuleDbContext : PostgresDbContext
{
    public YourModuleDbContext(DbContextOptions<YourModuleDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add your entity configurations
    }
}
```

3. Register the module in your application:
```csharp
services.AddDbContext<YourModuleDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
```

## Best Practices

1. **Vector Operations**:
   - Use appropriate vector dimensions for your embeddings
   - Consider using cosine similarity for text embeddings
   - Use Euclidean distance for spatial data

2. **Graph Operations**:
   - Keep graph queries simple and focused
   - Use appropriate indexes for graph traversal
   - Consider caching frequently accessed paths

3. **Module Design**:
   - Keep modules loosely coupled
   - Use the platform layer for shared functionality
   - Follow the layered architecture pattern

4. **Database Migrations**:
   - Always include extension checks in migrations
   - Use appropriate transaction handling
   - Follow PostgreSQL best practices for extensions

## Troubleshooting

### Common Issues

1. **Vector Operations Not Working**:
   - Ensure pgvector extension is installed
   - Check vector dimensions match your data
   - Verify connection string is correct

2. **Graph Operations Not Working**:
   - Ensure Apache AGE extension is installed
   - Check graph query syntax
   - Verify node and relationship types exist

3. **Database Connection Issues**:
   - Check connection string
   - Verify PostgreSQL is running
   - Check user permissions

4. **Migration Issues**:
   - Ensure extensions are installed before running migrations
   - Check user has superuser privileges for extension operations
   - Verify migration order is correct

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

# CAP Integration (Outbox/Inbox/Event Bus)

This template uses [CAP](https://github.com/dotnetcore/CAP) for reliable distributed messaging, outbox/inbox patterns, and event-driven architecture.

## CAP Features
- Outbox pattern: ensures events are only published if the local transaction commits
- Inbox pattern: ensures idempotent event processing
- Supports RabbitMQ, Kafka, PostgreSQL, and more
- Built-in dashboard for monitoring

## NuGet Packages
- DotNetCore.CAP
- DotNetCore.CAP.RabbitMQ
- DotNetCore.CAP.PostgreSql

## Configuration Example

In your API or Infrastructure project:

```csharp
services.AddCap(x =>
{
    x.UsePostgreSql(Configuration.GetConnectionString("DefaultConnection"));
    x.UseRabbitMQ("rabbitmq", 5672, "guest", "guest");
    x.UseDashboard();
});
```

## Publishing Events (Outbox)

```csharp
public class CapEventPublisher
{
    private readonly ICapPublisher _capBus;
    public CapEventPublisher(ICapPublisher capBus) => _capBus = capBus;

    public async Task PublishExampleEventAsync()
    {
        await _capBus.PublishAsync("example.event", new { Message = "Hello from CAP!" });
    }
}
```

## Subscribing to Events (Inbox)

```csharp
public class CapEventSubscriber
{
    [CapSubscribe("example.event")]
    public void HandleExampleEvent(dynamic data)
    {
        // Handle the event (inbox pattern)
        Console.WriteLine($"Received event: {data.Message}");
    }
}
```

## Dashboard
- Access the CAP dashboard at `http://localhost:8080/cap` (or wherever your API is running)

## Database
- CAP will automatically create its own tables for outbox/inbox management (e.g., cap.published, cap.received)

## Docker Compose
- RabbitMQ is included in the default `docker-compose.yml` and will work out of the box with CAP. 

# Redis Integration

This template includes Redis support for distributed caching and CAP distributed locks.

## Docker Compose
- Redis is included and available at `redis:6379`.

## NuGet Packages
- Microsoft.Extensions.Caching.StackExchangeRedis
- StackExchange.Redis

## Configuration Example

In your API or Infrastructure project:

```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration.GetConnectionString("Redis");
    options.InstanceName = "modulith:";
});
```

In your `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Redis": "redis:6379"
  }
}
```

## Using Redis Cache

```csharp
public class RedisCacheExampleService
{
    private readonly IDistributedCache _cache;
    public RedisCacheExampleService(IDistributedCache cache) => _cache = cache;

    public async Task SetValueAsync(string key, string value)
    {
        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    public async Task<string?> GetValueAsync(string key)
    {
        return await _cache.GetStringAsync(key);
    }
}
```

## CAP Redis Lock Support

To enable distributed locks for CAP:

```csharp
services.AddCap(x =>
{
    // ...
    x.UseRedisLock(Configuration.GetConnectionString("Redis"));
});
```

# Troubleshooting & FAQ

## RabbitMQ + CAP

**Q: My events are not being delivered or processed.**
- Check that RabbitMQ is running (`docker-compose ps` should show it as healthy).
- Check the CAP dashboard (`/cap`) for errors or retries.
- Ensure your event names match between publisher and subscriber.
- Check the RabbitMQ management UI (http://localhost:15672) for queues and messages.
- Make sure your API can resolve the `rabbitmq` hostname (works in Docker Compose by default).

**Q: I see connection errors to RabbitMQ.**
- Ensure the connection string/host/port/user/password in your CAP config matches your RabbitMQ setup.
- If running locally, try restarting Docker Compose.
- Check for port conflicts on 5672 or 15672.

**Q: Messages are stuck in the queue.**
- Check the CAP dashboard for consumer errors or retries.
- Ensure your subscriber method is decorated with `[CapSubscribe("event.name")]` and is public.
- Check for exceptions in your subscriber code.

**Q: CAP tables are missing in the database.**
- CAP should auto-create its tables (e.g., cap.published, cap.received) on startup.
- Ensure your database user has permission to create tables.
- Check your connection string and database availability.

**Q: The CAP dashboard is not available.**
- Ensure your API is running and accessible at the expected port.
- Check for errors in the API logs.
- The dashboard is at `/cap` (e.g., http://localhost:8080/cap).

## General Distributed Messaging Troubleshooting

- **Check all service logs**: Use `docker-compose logs -f` to see real-time logs for all services.
- **Network issues**: Ensure all services are on the same Docker network (handled by Compose).
- **Database issues**: Ensure PostgreSQL is running and accessible; check for migration errors.
- **Redis issues**: Ensure Redis is running and accessible; check for connection errors in logs.
- **CAP retries**: CAP will retry failed messages; check the dashboard for retry counts and errors.
- **Event versioning**: If you change event payloads, ensure all consumers are updated accordingly.
- **CAP configuration**: Double-check all connection strings and CAP options in your configuration.

---

# Logging & Observability

## Logging
- Uses Serilog for structured logging.
- Logs sent to Seq (http://localhost:5341), console, and Sentry.

## Error Tracking
- Sentry integration for error and exception monitoring.
- Configure your Sentry DSN in `appsettings.json`.

## Metrics & Dashboards
- Prometheus scrapes metrics from your .NET app at `/metrics`.
- Grafana (http://localhost:3000, password: admin) for dashboards and visualization.
- Prometheus config in `prometheus.yml`.

## Configuration
- See `appsettings.json` for Seq and Sentry URLs.
- See `ObservabilityConfig.cs` for Serilog and Prometheus setup.

## Usage
- Use `ILogger<T>` or Serilog's static `Log` class for logging.
- All logs and errors are available in Seq and Sentry.
- Metrics are available in Prometheus and Grafana.

---

# Grafana Dashboards

## Pre-provisioned Dashboards
- You can add JSON dashboard files to a `grafana/provisioning/dashboards` directory and reference them in your Grafana config.
- Example dashboards: HTTP request rates, error rates, business KPIs, CAP event metrics.
- See Grafana docs for [provisioning dashboards](https://grafana.com/docs/grafana/latest/administration/provisioning/#dashboards).

# Sentry Advanced Usage

## Custom Events & Context
- Add user context, breadcrumbs, and custom events to Sentry using `SentrySdk.ConfigureScope` and `SentrySdk.CaptureMessage`.
- Example in `ObservabilityExampleService`.

## Performance Monitoring
- Use Sentry transactions and spans to track performance of business operations.
- Example in `ObservabilityExampleService`.

# More Prometheus Metrics

## Custom Metrics
- Add business KPIs, CAP event metrics, and technical metrics using `prometheus-net`.
- Example counters and histograms in `ObservabilityExampleService`.

# Health Checks

## .NET Health Checks
- Register health checks in your API using `services.AddHealthChecks()`.
- Forward health check results to Prometheus with `.ForwardToPrometheus()`.
- Add custom health checks by implementing `IHealthCheck` (see `ExampleHealthCheck`).
- Health check metrics will be available at `/metrics` and can be visualized in Grafana.

---

# Per-Module Grafana Dashboards

## Metric Labels
- All Prometheus metrics use a `module` label for per-module identification.
- Example: `business_event_counter{module="users"}`

## Dashboard Provisioning
- Dashboards for each module are stored in `grafana/provisioning/dashboards/`.
- Provisioning config is in `grafana/provisioning/dashboards.yml`.
- Example dashboards: `users-dashboard.json`, `payments-dashboard.json`.
- Grafana will automatically load and update dashboards for each module.

## How to Add a New Module Dashboard
1. Create a new dashboard in Grafana and export it as JSON.
2. Save the JSON file in `grafana/provisioning/dashboards/` (e.g., `orders-dashboard.json`).
3. Use the `module` label in your Prometheus queries to filter metrics for the new module.

---

# Per-Module Database Schema

## Schema per Module
- Each module uses its own schema in the relational database (e.g., PostgreSQL).
- This is configured in each module's `DbContext` using `modelBuilder.HasDefaultSchema("modulename")`.

## Example

```csharp
public class UsersDbContext : PostgresDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("users");
        // ... entity configs ...
    }
}
```

## Benefits
- Isolation and clarity for each module's tables
- Easier migration to microservices
- Security and permission management per schema

## How to Add a New Module Schema
1. In your module's `DbContext`, set the default schema to the module name.
2. Run migrations; tables will be created under the correct schema.
3. Use the schema name in queries if needed (e.g., `users.Users`).

---

# Cross-Cutting Concerns

## Shared Infrastructure
- Centralized logging, distributed cache, CAP for messaging, and other cross-cutting services are implemented in shared projects (e.g., SharedKernel).
- Register these services in the DI container and use them in all modules.

## Example Middleware
- Use middleware for concerns like correlation ID propagation, logging, error handling, etc.
- Example: `CorrelationIdMiddleware` sets and propagates a correlation ID for each request.

## Example Shared Services
- Place reusable services (e.g., `ICorrelationIdProvider`) in the Shared Kernel.
- Register implementations (e.g., `CorrelationIdProvider`) in the DI container.
- Inject and use in any module or middleware.

## MediatR Pipeline Behaviors
- Use MediatR pipeline behaviors for validation, logging, and other cross-cutting concerns in CQRS/mediator patterns.
- Example: `ValidationBehavior` (in Shared Kernel) and `MediatRLoggingBehavior` (in API) are registered with MediatR and run for every request.

## How to Use in Modules
1. Register shared services and pipeline behaviors in your API or module startup:
   ```csharp
   services.AddSingleton<ICorrelationIdProvider, CorrelationIdProvider>();
   services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
   services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatRLoggingBehavior<,>));
   app.UseMiddleware<CorrelationIdMiddleware>();
   ```
2. Inject and use shared services in your modules as needed.
3. Add or extend middleware and pipeline behaviors for new cross-cutting concerns.

---

# CAP Configuration: Shared Default with Module Override

## Shared Default CAP Configuration
- The shared kernel provides a default CAP configuration via `AddSharedCap`.
- All modules use this by default for consistent messaging and outbox/inbox tables.

## Module-Level Override
- Any module can override the default CAP configuration by calling `AddCapServices(..., useDefaultCap: false)` and providing its own settings (e.g., different DB, schema, or broker).
- This allows for full isolation and easy migration to microservices.

## Table/Schema Isolation
- By default, CAP tables are created in the schema/database you configure.
- You can use `x.UseSchema("modulename")` or set a table prefix for further isolation.

## Example Usage

```csharp
// In module startup/configuration
services.AddCapServices(configuration); // Uses shared CAP config

// To override in a module:
services.AddCapServices(configuration, useDefaultCap: false); // Uses module-specific CAP config
```

---

# Blueprint Example: Todo

This template includes a full Todo example as a blueprint for building modules:

- **Domain:** `TodoItem` entity with business logic, nested `TodoDetails`, and `TodoList` with a collection of items.
- **Application:** MediatR command/query handlers for adding and retrieving todos, with FluentValidation for validation, CAP event publishing, and Redis caching.
- **Infrastructure:** DbContext with schema and entity registration.
- **API:** Controller with endpoints for CRUD operations, a CAP event subscriber, DTOs, manual mappers, and AutoMapper profile (including advanced mapping scenarios: nested objects, collections, custom value resolvers).
- **Tests:** Unit and integration tests for entity, command handler, CAP event publishing, Redis caching, validation, DTO mapping, AutoMapper profile, and advanced mapping scenarios.

## How to Use/Extend
- Use the Todo example as a starting point for your own modules.
- Copy and adapt the entity, command, handler, controller, DTO, and mapping patterns.
- Add more business logic, validation, event publishing, caching, mapping (including advanced scenarios), and tests as needed.
- See the `Tests` folder for examples of unit and integration testing with xUnit, FluentAssertions, NSubstitute, and AutoMapper.

---

# CQRS Pattern

This template uses the CQRS (Command Query Responsibility Segregation) pattern:

- **Commands**: Change state (write). Example: `AddTodoCommand`.
- **Queries**: Read data (read). Example: `GetTodosQuery`.
- **Handlers**: Each command/query has its own handler.
- **MediatR**: Used to dispatch commands and queries, enforcing separation.

## How to Add a Command or Query
1. Create a new command or query class in the appropriate folder.
2. Implement a handler for it.
3. Register any validators as needed.
4. Use MediatR in your API/controller to send commands/queries.

## Example
- `AddTodoCommand` (write) and `GetTodosQuery` (read) in the Todo blueprint.

# Rich Domain Models (No Anemic Models)

- Domain models should encapsulate business logic and invariants, not just data.
- Use methods on your entities/aggregates to perform operations (e.g., `MarkComplete()` on `TodoItem`).
- Avoid "anemic" models (just properties, no behavior).
- Place business rules and logic in the domain layer, not in handlers or services.

## Domain Tests
- Write unit tests for your domain models to ensure business rules are enforced.
- See `TodoItemTests` for an example of testing domain behavior.

---

# Architecture Enforcement

This template uses [ArchUnit.NET](https://www.archunit.net/) to enforce architectural rules as automated tests. This helps prevent architectural degradation over time and ensures that the modular structure is maintained.

## How it Works
- Architectural rules are defined as XUnit tests in your test projects (e.g., `Modulith.NewModule.Tests/Architecture/ArchitectureTests.cs`).
- These tests run during your build process and will fail if any code violates the defined rules.

## Example Rules
- **Layer Dependencies**: Ensures that layers only depend on allowed layers (e.g., Domain not depending on Infrastructure).
- **No Circular Dependencies**: Prevents cyclic references between projects.

## How to Add/Modify Rules
1. Open the `ArchitectureTests.cs` file in your module's test project.
2. Define new rules or modify existing ones using ArchUnit.NET's fluent API.
3. Run your tests to check for architectural compliance.

---

### Global Error Handling / Problem Details

The template incorporates global error handling using the **RFC 7807 Problem Details** specification. This ensures that all unhandled exceptions and API errors return consistent, machine-readable responses.

-   **`GlobalExceptionHandlingMiddleware`**: This custom middleware catches unhandled exceptions, logs them, and transforms them into `ProblemDetails` objects.
-   **Consistent Responses**: All API errors will provide structured `ProblemDetails` with details like `type`, `title`, `status`, and `instance` (request path). In development, `detail` (exception message) and `stackTrace` are also included for easier debugging.

This setup provides a standardized way for API consumers to understand and handle errors programmatically.

### API Versioning

The template includes built-in support for API versioning using the `Microsoft.AspNetCore.Mvc.Versioning` package. This allows you to maintain multiple versions of your API endpoints while providing a clear upgrade path for consumers.

#### Features

- **Multiple Version Support**: APIs can be versioned using URL segments (e.g., `/api/v1/todos`), headers, or media types
- **Default Version**: Version 1.0 is set as the default when no version is specified
- **Version Reporting**: API versions are reported in response headers
- **Swagger Integration**: API versions are properly documented in Swagger UI

#### Versioning Strategies

1. **URL Segment Versioning**:
   ```http
   GET /api/v1/todos
   GET /api/v2/todos
   ```

2. **Header Versioning**:
   ```http
   GET /api/todos
   x-api-version: 2.0
   ```

3. **Media Type Versioning**:
   ```http
   GET /api/todos
   Accept: application/json;x-api-version=2.0
   ```

#### Implementation Example

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/todos")]
public class TodoController : ControllerBase
{
    // V1 implementation
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/todos")]
public class TodoController : ControllerBase
{
    // V2 implementation with additional features
}
```

#### Best Practices

1. **Version Lifecycle**:
   - Keep old versions until all consumers have migrated
   - Document deprecation timelines
   - Use semantic versioning (MAJOR.MINOR)

2. **Breaking Changes**:
   - Major version changes for breaking changes
   - Minor version changes for new features
   - Maintain backward compatibility within major versions

3. **Documentation**:
   - Clearly document version differences
   - Provide migration guides
   - Include version information in API responses

### Health Checks UI

The template includes a comprehensive health monitoring system using `AspNetCore.HealthChecks.UI`. This provides a real-time dashboard to monitor the health of various components in your system.

#### Features

- **Real-time Monitoring**: Live dashboard showing the health status of all components
- **Multiple Health Checks**:
  - Database (PostgreSQL)
  - Redis Cache
  - RabbitMQ
  - External Services
  - Custom Health Checks
- **Historical Data**: Track health status over time
- **Customizable UI**: Styled dashboard with custom CSS
- **REST API**: Programmatic access to health check results

#### Endpoints

- **Health Check UI**: `/health-ui` - Interactive dashboard
- **Health Check API**: `/health` - JSON response with health status
- **Health Check API UI**: `/health-api` - API documentation

#### Configuration

Health checks are configured in `appsettings.json`:

```json
{
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "NewModule API",
        "Uri": "/health"
      }
    ],
    "EvaluationTimeInSeconds": 15,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}
```

#### Adding Custom Health Checks

1. Create a new health check class:

```csharp
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Implement your health check logic
        return Task.FromResult(HealthCheckResult.Healthy("Custom check is healthy"));
    }
}
```

2. Register the health check in `HealthChecksConfig.cs`:

```csharp
services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("Custom");
```

#### Best Practices

1. **Check Critical Dependencies**:
   - Database connections
   - Message queues
   - External services
   - File system access
   - Memory usage

2. **Set Appropriate Timeouts**:
   - Configure reasonable timeouts for each check
   - Consider the impact on system performance

3. **Monitor Health Check Results**:
   - Set up alerts for unhealthy states
   - Track health check history
   - Use the data for capacity planning

4. **Security**:
   - Secure health check endpoints in production
   - Use appropriate authentication
   - Limit access to sensitive health data

# ... existing code ... 