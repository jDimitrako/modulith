# Modulith Template

A modern, scalable template for building modular monolith applications with advanced PostgreSQL capabilities including vector search and graph operations.

## Features

- **Modular Architecture**
  - Clean separation of concerns
  - Independent module development
  - Easy module integration
  - Domain-driven design principles

- **Advanced PostgreSQL Features**
  - Vector search with pgvector
  - Graph operations with Apache AGE
  - Full-text search capabilities
  - JSON/JSONB support

- **Modern Tech Stack**
  - .NET 8.0
  - PostgreSQL 15+
  - Redis for caching
  - RabbitMQ for messaging
  - Seq for logging

- **Development Tools**
  - Docker & Docker Compose support
  - Hot reload enabled
  - Comprehensive logging
  - API documentation with Swagger

## Adding a New Module

To add a new module to the application, follow these steps:

1. **Create Module Structure**
   ```
   YourModule/
   ├── Modulith.YourModule/
   │   ├── Controllers/
   │   ├── Services/
   │   └── Models/
   ├── Modulith.YourModule.Contracts/
   │   └── DTOs/
   └── Modulith.YourModule.Tests/
   ```

2. **Add Module Project References**
   - Add your module projects to the solution
   - Reference the module in Modulith.API.csproj:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\YourModule\Modulith.YourModule\Modulith.YourModule.csproj" />
   </ItemGroup>
   ```

3. **Create Module Service Registrar**
   ```csharp
   // YourModule/Modulith.YourModule/YourModuleServiceRegistrar.cs
   public static class YourModuleServiceRegistrar
   {
       public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
       {
           // Register your module's services
           services.AddScoped<IYourModuleService, YourModuleService>();
           
           // Add any module-specific configuration
           services.Configure<YourModuleOptions>(configuration.GetSection("YourModule"));
       }
   }
   ```

4. **Register Module in Bootstrapper**
   - Open `Modulith.API/Program.cs`
   - Add your module registration:
   ```csharp
   // Register modules
   YourModuleServiceRegistrar.ConfigureServices(builder.Services, builder.Configuration);
   ```

5. **Add Module Configuration**
   - Add your module's configuration to `appsettings.json`:
   ```json
   {
     "YourModule": {
       "Option1": "value1",
       "Option2": "value2"
     }
   }
   ```

6. **Add Module to Docker Compose** (if needed)
   ```yaml
   yourmoduleapi:
     build:
       context: .
       dockerfile: YourModule/Modulith.YourModule/Dockerfile
     container_name: yourmoduleapi
     environment:
       - ASPNETCORE_ENVIRONMENT=Development
       - ConnectionStrings__Postgres=Host=postgres;Port=5432;Database=modulith_db;Username=admin;Password=admin
     ports:
       - "8082:8080"
     depends_on:
       - postgres
   ```

## Best Practices

1. **Module Independence**
   - Keep modules loosely coupled
   - Use contracts for inter-module communication
   - Avoid direct dependencies between modules

2. **Configuration**
   - Use module-specific configuration sections
   - Keep sensitive data in user secrets or environment variables
   - Use strongly-typed configuration objects

3. **API Design**
   - Use consistent URL patterns
   - Version your APIs
   - Document with Swagger/OpenAPI

4. **Testing**
   - Write unit tests for each module
   - Include integration tests
   - Test module interactions

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL 15+](https://www.postgresql.org/download/) (if running locally)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## Quick Start

### Using Docker (Recommended)

1. Clone the repository:
```bash
git clone https://github.com/yourusername/modulith-template.git
cd modulith-template
```

2. Start all services:
```bash
docker-compose up --build
```

3. Access the services:
- Main API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- RabbitMQ UI: http://localhost:15672 (guest/guest)
- Seq (logs): http://localhost:5341

### Manual Setup

1. Install PostgreSQL extensions:
```sql
CREATE EXTENSION vector;
CREATE EXTENSION age;
```

2. Update connection strings in `appsettings.json`

3. Run the application:
```bash
dotnet run --project Modulith.Web
```

## Project Structure

```
Modulith/
├── src/
│   ├── Modules/
│   │   ├── ModuleName/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   ├── Infrastructure/
│   │   │   └── Api/
│   └── Platform/
│       ├── Common/
│       ├── Infrastructure/
│       └── Api/
└── tests/
    └── Modules/
        └── ModuleName/
```

## Configuration

### Environment Variables

Key environment variables:
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Database=modulith;Username=postgres;Password=postgres
Redis__ConnectionString=localhost:6379
RabbitMQ__Host=localhost
```

### Database Configuration

1. Connection string format:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=your_database;Username=your_username;Password=your_password"
  }
}
```

2. Required PostgreSQL extensions:
```sql
CREATE EXTENSION vector;
CREATE EXTENSION age;
```

## Development Guide

### Creating a New Module

1. Use the template:
```bash
dotnet new module -n YourModuleName
```

2. Add module reference to the main project:
```xml
<ProjectReference Include="..\YourModuleName\YourModuleName.Api\YourModuleName.Api.csproj" />
```

### Database Migrations

1. Add a migration:
```bash
dotnet ef migrations add InitialCreate --project YourModule.Infrastructure --startup-project YourModule.Api
```

2. Apply migrations:
```bash
dotnet ef database update --project YourModule.Infrastructure --startup-project YourModule.Api
```

### Vector Search Example

```csharp
// Find similar documents
var similarDocs = await dbContext.Documents
    .OrderByCosineDistance(x => x.Embedding, queryVector)
    .Take(10)
    .ToListAsync();
```

### Graph Operations Example

```csharp
// Find paths between nodes
var paths = await dbContext.ExecuteGraphQueryAsync<Path>(
    "MATCH p=(a:Person)-[:FRIEND*]->(b:Person) WHERE a.name = $name RETURN p",
    new { name = "John" });
```

## Testing

1. Run unit tests:
```bash
dotnet test
```

2. Run integration tests:
```bash
dotnet test --filter Category=Integration
```

## Monitoring & Logging

- **Seq**: http://localhost:5341
- **Health Checks**: http://localhost:8080/health
- **Metrics**: http://localhost:8080/metrics

## Security

- JWT authentication
- Role-based authorization
- HTTPS enabled
- Secure headers
- CORS configuration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [pgvector](https://github.com/pgvector/pgvector)
- [Apache AGE](https://age.apache.org/)
- [.NET](https://dotnet.microsoft.com/)