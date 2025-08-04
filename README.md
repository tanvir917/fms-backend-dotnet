# Care Management System - ASP.NET Core Microservices Backend

This is the ASP.NET Core microservices backend for the Care Management System, designed to work alongside the existing Django backend during the transition period.

## Architecture

This solution follows a microservices architecture with the following components:

### 🎯 Core Services

- **API Gateway** (Port 5000) - Ocelot-based gateway for routing requests
- **Authentication Service** (Port 5001) - User authentication and JWT token management
- **Staff Service** (Port 5002) - Staff member management and operations
- **Clients Service** (Port 5003) - Client management (to be implemented)
- **Roster Service** (Port 5004) - Shift scheduling and roster management (to be implemented)
- **Billing Service** (Port 5005) - Billing and invoicing (to be implemented)

### 🛠 Infrastructure

- **PostgreSQL** (Port 5433) - Database with separate schemas per service
- **RabbitMQ** (Port 5673) - Message broker for inter-service communication
- **Shared Library** - Common DTOs, events, and utilities

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- PostgreSQL (if running outside Docker)

### Running with Docker

1. **Build and start all services:**

   ```bash
   cd backend-dotnet
   docker compose up --build
   ```

2. **Access the services:**
   - API Gateway: http://localhost:5000
   - Auth API: http://localhost:5001
   - Staff API: http://localhost:5002
   - RabbitMQ Management: http://localhost:15673 (guest/guest)

### Running Locally (Development)

1. **Start infrastructure services:**

   ```bash
   docker compose up postgres-dotnet rabbitmq-dotnet
   ```

2. **Run each service individually:**

   ```bash
   # Authentication Service
   cd src/Services/Auth/CareManagement.Auth.Api
   dotnet run

   # Staff Service
   cd src/Services/Staff/CareManagement.Staff.Api
   dotnet run

   # API Gateway
   cd src/Gateway/CareManagement.Gateway
   dotnet run
   ```

## 🔧 Configuration

### Database Connections

Each service connects to its own PostgreSQL database:

- Auth Service: `CareManagement_Auth`
- Staff Service: `CareManagement_Staff`
- Clients Service: `CareManagement_Clients`
- Roster Service: `CareManagement_Roster`
- Billing Service: `CareManagement_Billing`

### JWT Configuration

All services share the same JWT configuration for seamless authentication:

```json
{
  "JWT": {
    "Key": "YourVeryLongSecretKeyThatIsAtLeast256BitsLong12345",
    "Issuer": "CareManagement.Api",
    "Audience": "CareManagement.Client"
  }
}
```

## 📡 API Endpoints

### Authentication Service

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/change-password` - Change password
- `GET /api/auth/users` - List all users (Admin only)

### Staff Service

- `GET /api/staff` - List staff members (with pagination)
- `GET /api/staff/{id}` - Get specific staff member
- `POST /api/staff` - Create new staff member
- `PUT /api/staff/{id}` - Update staff member
- `GET /api/staff/stats` - Get staff statistics
- `GET /api/staff/leave-requests` - Get leave requests

## 🔐 Authentication & Authorization

The system uses JWT Bearer tokens for authentication. Users must include the token in the Authorization header:

```
Authorization: Bearer <jwt-token>
```

### Roles

- **Admin** - Full system access
- **Care_Coordinator** - Manage staff and clients
- **Carer** - Limited access to assigned tasks

## 🌐 Frontend Integration

Update your frontend API configuration to use the new .NET backend:

```typescript
// For testing the .NET backend
const API_BASE_URL = "http://localhost:5000/api";

// Keep Django backend for comparison
const DJANGO_API_BASE_URL = "http://localhost:8000/api";
```

## 📊 Message Bus Events

The system publishes events for inter-service communication:

- `UserCreatedEvent` - When a new user is registered
- `StaffCreatedEvent` - When a new staff member is added
- `ClientCreatedEvent` - When a new client is added
- `ShiftCreatedEvent` - When a new shift is scheduled

## 🏗 Development Status

### ✅ Completed

- [x] Solution structure and shared libraries
- [x] API Gateway with Ocelot
- [x] Authentication service with JWT
- [x] Staff service with CRUD operations
- [x] Docker containerization
- [x] RabbitMQ message bus integration

### 🔄 In Progress

- [ ] Clients service implementation
- [ ] Roster service implementation
- [ ] Billing service implementation

### 📋 Planned

- [ ] Event sourcing implementation
- [ ] API versioning
- [ ] Health checks and monitoring
- [ ] Unit and integration tests
- [ ] API documentation with Swagger

## 🧪 Testing Both Backends

During the transition period, you can test both backends:

1. **Django Backend**: http://localhost:8000
2. **ASP.NET Core Backend**: http://localhost:5000

Both backends provide the same functionality, allowing for feature-by-feature migration and comparison.

## 🛠 Building and Deployment

### Build Solution

```bash
dotnet build CareManagement.sln
```

### Run Tests

```bash
dotnet test
```

### Publish for Production

```bash
dotnet publish -c Release
```

## 📝 Next Steps

1. **Complete remaining microservices** (Clients, Roster, Billing)
2. **Implement comprehensive testing**
3. **Add monitoring and logging**
4. **Set up CI/CD pipeline**
5. **Gradually migrate frontend to use .NET backend**
6. **Sunset Django backend once migration is complete**
