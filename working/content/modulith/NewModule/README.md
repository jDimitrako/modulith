# NewModule

A modular implementation following Domain-Driven Design principles.

## Features

- Domain-driven design architecture
- Event-driven communication with other modules
- Integration with User module
- Health checks and metrics
- Database migrations
- CI/CD pipeline

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL
- RabbitMQ

## Getting Started

1. **Database Setup**
   ```bash
   cd scripts
   chmod +x migrate-db.sh
   ./migrate-db.sh
   ```

2. **Configuration**
   Update `appsettings.json` with your environment-specific settings:
   ```json
   {
     "ConnectionStrings": {
       "NewModuleDb": "Host=localhost;Database=newmodule_db;Username=postgres;Password=postgres"
     },
     "RabbitMQ": {
       "HostName": "localhost",
       "UserName": "guest",
       "Password": "guest",
       "VirtualHost": "/"
     }
   }
   ```

3. **Running the Module**
   ```bash
   dotnet run --project Modulith.NewModule.Api
   ```

## Module Structure

```
NewModule/
├── Modulith.NewModule.Api/           # API layer
├── Modulith.NewModule.Application/   # Application layer
├── Modulith.NewModule.Domain/        # Domain layer
├── Modulith.NewModule.Infrastructure/# Infrastructure layer
├── Modulith.NewModule.Tests/         # Tests
├── scripts/                          # Development scripts
└── templates/                        # Code generation templates
```

## Integration Events

The module subscribes to the following events from the User module:
- `UserCreatedEvent`

## Health Checks

Available health check endpoints:
- `/health` - Overall health
- `/health/newmodule-db` - Database health
- `/health/newmodule-health` - Module-specific health

## Metrics

Available metrics:
- `newmodule.users.created` - Counter for created users
- `newmodule.users.updated` - Counter for updated users
- `newmodule.users.operation.duration` - Histogram for operation durations
- `newmodule.users.active` - Gauge for active users

## Development

### Code Generation

Generate new entities:
```bash
cd scripts
chmod +x generate-entity.sh
./generate-entity.sh NewEntityName
```

### Testing

Run tests:
```bash
dotnet test
```

## CI/CD

The module includes a GitHub Actions workflow for:
- Building
- Testing
- Database migrations
- Deployment (to be configured)

## Contributing

1. Create a feature branch
2. Make your changes
3. Run tests
4. Submit a pull request

## License

This project is licensed under the MIT License. 