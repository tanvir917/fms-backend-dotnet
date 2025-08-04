# Care Management System - .NET Backend Makefile

.PHONY: help build up down logs clean restore test

# Default help target
help:
	@echo "Care Management System - .NET Backend Commands"
	@echo "=============================================="
	@echo "  build         Build the solution"
	@echo "  restore       Restore NuGet packages"
	@echo "  up            Start all services with Docker"
	@echo "  up-infra      Start only infrastructure (PostgreSQL, RabbitMQ)"
	@echo "  down          Stop all Docker services"
	@echo "  logs          View Docker logs"
	@echo "  clean         Clean build artifacts"
	@echo "  test          Run tests"
	@echo "  auth          Run Auth service locally"
	@echo "  staff         Run Staff service locally"
	@echo "  gateway       Run API Gateway locally"
	@echo ""

# Build the solution
build:
	dotnet build CareManagement.sln

# Restore NuGet packages
restore:
	dotnet restore CareManagement.sln

# Start all services with Docker Compose
up:
	docker compose up --build

# Start all services with Docker Compose in detached mode
up-detached:
	docker compose up --build -d

# Start only infrastructure services (PostgreSQL, RabbitMQ)
up-infra:
	docker compose up postgres-dotnet rabbitmq-dotnet -d

# Stop all Docker services
down:
	docker compose down

# View Docker logs
logs:
	docker compose logs -f

# Clean build artifacts
clean:
	dotnet clean CareManagement.sln
	docker compose down --volumes --remove-orphans

# Run tests
test:
	dotnet test CareManagement.sln

# Run individual services locally (requires infrastructure to be running)
auth:
	cd src/Services/Auth/CareManagement.Auth.Api && dotnet run

staff:
	cd src/Services/Staff/CareManagement.Staff.Api && dotnet run

gateway:
	cd src/Gateway/CareManagement.Gateway && dotnet run

# Development workflow shortcuts
dev-setup: up-infra
	@echo "Infrastructure started. You can now run individual services with:"
	@echo "  make auth"
	@echo "  make staff"
	@echo "  make gateway"

# Production-like local testing
prod-test: up-detached
	@echo "All services started in detached mode"
	@echo "Access points:"
	@echo "  API Gateway: http://localhost:5003"
	@echo "  Auth API: http://localhost:5001"
	@echo "  Staff API: http://localhost:5002"
	@echo "  RabbitMQ Management: http://localhost:15673"

# Quick restart
restart: down up-detached
