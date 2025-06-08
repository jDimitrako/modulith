#!/bin/bash

# Start PostgreSQL and RabbitMQ
docker-compose up -d

# Wait for services to be ready
echo "Waiting for services to be ready..."
sleep 10

# Check if services are running
if docker-compose ps | grep -q "Up"; then
    echo "Services are running!"
    echo "PostgreSQL is available at localhost:5432"
    echo "RabbitMQ Management UI is available at http://localhost:15672"
    echo "RabbitMQ credentials: guest/guest"
else
    echo "Error: Services failed to start"
    exit 1
fi 