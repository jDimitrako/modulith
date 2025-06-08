# Modulith Template

A template for creating modular monolith applications with PostgreSQL, vector search, and graph capabilities.

## Features

- Modular monolith architecture
- PostgreSQL with pgvector for vector search
- Apache AGE for graph operations
- Layered architecture (Domain, Application, Infrastructure, Api)
- Platform services for shared functionality

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