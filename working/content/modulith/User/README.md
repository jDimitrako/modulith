# User Module

The User module provides authentication, authorization, and user management functionality for the Modulith application. It follows a modular architecture and integrates with other modules through events and shared authorization policies.

## Features

### Authentication
- JWT-based authentication
- Password-based login
- External authentication providers (Google, Microsoft)
- Refresh token support
- Remember me functionality

### Authorization
- Role-based access control
- Custom authorization policies
- User status verification
- Email verification status

### User Management
- User registration
- Profile management
- Account activation/deactivation
- Role management
- External account linking

### Event Integration
- Event-based communication with other modules
- User lifecycle events
- Role change events
- Account status events

## Architecture

The module follows a clean architecture pattern with the following layers:

### Domain Layer (`Modulith.User.Domain`)
- Entities (User)
- Domain Events
- Value Objects
- Domain Interfaces

### Application Layer (`Modulith.User.Application`)
- Commands and Queries
- Command/Query Handlers
- Domain Event Handlers
- Application Services

### Infrastructure Layer (`Modulith.User.Infrastructure`)
- Persistence (UserDbContext)
- External Service Integration
- Authorization Handlers
- JWT Token Generation

### API Layer (`Modulith.User.Api`)
- Controllers
- Service Registration
- API Endpoints
- Middleware Configuration

### Contracts (`Modulith.User.Contracts`)
- DTOs
- Authorization Policies
- Shared Interfaces

## Integration Points

### Event Bus
The module uses CAP (Event Bus) with RabbitMQ for event-based communication:

```csharp
// Subscribe to user events in other modules
[CapSubscribe("user.created")]
public async Task HandleUserCreated(UserCreatedEvent @event)
{
    // Handle user created event
}
```

### Authorization
Other modules can use the shared authorization policies:

```csharp
[Authorize(Policy = UserPolicies.RequireAdminRole)]
public IActionResult AdminOnly()
{
    // Only accessible by admin users
}

[Authorize(Policy = UserPolicies.RequireActiveUser)]
public IActionResult ActiveUsersOnly()
{
    // Only accessible by active users
}
```

### User Information
Access user information in other modules:

```csharp
[Authorize]
public async Task<IActionResult> SomeAction()
{
    var user = await _userManager.GetUserAsync(User);
    // Use user information
}
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=modulith;Username=postgres;Password=postgres"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "Microsoft": {
      "ClientId": "your-microsoft-client-id",
      "ClientSecret": "your-microsoft-client-secret"
    }
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  },
  "UserModule": {
    "PasswordHashIterations": 10000,
    "RefreshTokenExpiryDays": 7,
    "RequireEmailConfirmation": true,
    "DefaultRoles": ["User"]
  }
}
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login with credentials
- `GET /api/auth/external-login` - Initiate external login
- `GET /api/auth/external-login-callback` - Handle external login callback

### User Management
- `GET /api/auth/me` - Get current user info
- `PUT /api/auth/profile` - Update user profile
- `POST /api/auth/change-password` - Change password
- `POST /api/auth/refresh-token` - Refresh JWT token

## Development

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL
- RabbitMQ

### Running Tests
```bash
dotnet test
```

### Database Migrations
```bash
dotnet ef migrations add InitialCreate --project Modulith.User.Infrastructure --startup-project Modulith.User.Api
dotnet ef database update --project Modulith.User.Infrastructure --startup-project Modulith.User.Api
```

## Contributing

1. Follow the modular architecture pattern
2. Add appropriate unit and integration tests
3. Update documentation for new features
4. Follow the existing code style and patterns

## License

This module is part of the Modulith template and follows its licensing terms. 