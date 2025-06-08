#!/bin/bash

# Build the project
dotnet build

# Run migrations
echo "Running database migrations..."
dotnet ef migrations add InitialCreate --project Modulith.User.Infrastructure --startup-project Modulith.User.Api
dotnet ef database update --project Modulith.User.Infrastructure --startup-project Modulith.User.Api

echo "Migrations completed!" 