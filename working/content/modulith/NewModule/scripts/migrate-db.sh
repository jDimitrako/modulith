#!/bin/bash

# Exit on error
set -e

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting database migration for NewModule...${NC}"

# Navigate to the Infrastructure project directory
cd "$(dirname "$0")/../Modulith.NewModule.Infrastructure"

# Remove existing migrations
echo -e "${GREEN}Cleaning up existing migrations...${NC}"
rm -rf Migrations/*

# Add new migration
echo -e "${GREEN}Creating new migration...${NC}"
dotnet ef migrations add InitialCreate --startup-project ../Modulith.NewModule.Api

# Update database
echo -e "${GREEN}Updating database...${NC}"
dotnet ef database update --startup-project ../Modulith.NewModule.Api

echo -e "${GREEN}Database migration completed successfully!${NC}" 