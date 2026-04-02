# Group-9-QLI-Pickup

## What is this application?

QLIFT is a pickup coordination system for QLI (Quality Living, Inc.) that helps manage transportation scheduling between clients and drivers.

## How to use

### Development Mode

### MySQL Startup
1. Log into MySQL and create the database:
```sql
   CREATE DATABASE qlipickup;
```
2.  Open `API/appsettings.json` and replace the password with your local MySQL root password
3.  Run the backend:
```bash
   cd API
   dotnet restore
   dotnet ef database update
   dotnet run
```

1. **Start the API (Backend)**
   ```bash
   cd API
   dotnet watch run
   ```
   API runs on `http://localhost:5270`

2. **Start the React App (Frontend)** - in a separate terminal
   ```bash
   cd Client
   npm install
   npm run dev
   ```
   Frontend runs on `http://localhost:5173`

### Production Mode

1. Build the React app:
   ```bash
   cd Client
   npm run build
   ```

2. Copy the build to the API:
   ```bash
   mkdir -p ../API/wwwroot
   cp -R dist/* ../API/wwwroot/
   ```

3. Run the API (serves both frontend and backend):
   ```bash
   cd ../API
   dotnet run
   ```
   Access the app at `http://localhost:5270`
   Visit `http://localhost:5270/swagger` to test the API endpoints

## Release Notes

### Current Submission - Transportation Management System (April 2, 2026)

**What's Working:**

#### ✅ Authentication & Authorization System
- **User Registration & Login** - JWT-based authentication with secure password hashing (BCrypt)
- **Role-Based Access Control** - Three user roles: Admin, Driver, and Rider
- **Admin Account Management** - Admins can create other admin and driver accounts
- **Token-Based Sessions** - JWT tokens with configurable expiration (60 minutes default)
- **Secure Endpoints** - All sensitive operations require authentication and proper role authorization

#### ✅ Transportation Types API (NEW - Fully Implemented)
Complete CRUD API for managing transportation type categories:

**Endpoints:**
- `POST /api/transportationtypes` - Create new transportation type (Admin only)
- `GET /api/transportationtypes` - List all active types (Authenticated users)
- `GET /api/transportationtypes?includeDeleted=true` - List including deleted (Admin only)
- `GET /api/transportationtypes/{id}` - Get specific type by ID (Authenticated users)
- `PUT /api/transportationtypes/{id}` - Update existing type (Admin only)
- `DELETE /api/transportationtypes/{id}` - Soft delete type (Admin only)

**Features:**
- ✅ Soft delete functionality (records never permanently deleted)
- ✅ Safety checks - prevents deletion if active trips exist
- ✅ Sort ordering support for display customization
- ✅ Active/inactive status management
- ✅ Data validation with clear error messages
- ✅ Service layer architecture for business logic separation
- ✅ Full integration with database via Entity Framework Core

**Testing:**
- All endpoints tested and verified working
- Authentication and authorization tested
- Soft delete functionality confirmed
- Database persistence verified
- See `API/TransportationTypes.http` for test examples
- See `API/TransportationTypes_API_Documentation.md` for complete API documentation

#### ✅ Database Models
**Implemented Models:**
- `User` - User accounts with roles (Admin, Driver, Rider)
- `TransportationType` - Transportation categories (Medical, Shopping, Personal, etc.)
- `Trip` - Trip scheduling and tracking (partial implementation)
- `Rider` - Rider profile information
- `Driver` - Driver profile information (structure in place)
- `Vehicle` - Vehicle information (structure in place)
- `SpecialDate` - Hours of operation special dates

#### ✅ Infrastructure
- ASP.NET Core API with controller support
- React + TypeScript frontend
- MySQL database with Entity Framework Core
- API-frontend communication (CORS configured)
- Swagger API documentation at `/swagger`
- Health check endpoint (`/api/ping`)
- Development environment with hot reload
- Production build and deployment setup

**What's In Progress:**
- Trip management endpoints (database models ready)
- Driver and Rider profile management endpoints
- Frontend UI components for transportation types
- Trip status tracking and history

**Known Issues:**
- Full migration system needs to be set up with Entity Framework tools
- Some controller placeholders (RiderController, DriverController) need full implementation
- TripStatusHistory and related models need integration

## Recent Changes (April 2, 2026)

### Models Added/Updated
1. **TransportationType Model** - Complete with soft delete support
   - Fields: Id, Label, Description, SortOrder, IsActive, IsDeleted
   - Relationship: One-to-Many with Trips
   
2. **SpecialDate Model** - Fixed syntax error (missing semicolon)

### API Endpoints Added
1. **Transportation Types Controller** (`/api/transportationtypes`)
   - Full CRUD operations
   - Admin-only create, update, delete
   - All authenticated users can view
   - Soft delete with safety checks

2. **Transportation Type Service** (`TransportationTypeService`)
   - Business logic layer
   - Safety validation for deletions
   - Query optimization with sorting

### DTOs Added
1. **TransportationTypeDTOs.cs**
   - `CreateTransportationTypeRequest` - For creating new types
   - `UpdateTransportationTypeRequest` - For updating existing types
   - `TransportationTypeResponse` - For API responses

### Configuration Updates
1. **Program.cs** - Registered `TransportationTypeService` in DI container
2. **AppDbContext.cs** - TransportationTypes DbSet already configured

### Documentation Added
1. **TransportationTypes.http** - HTTP test file with example requests
2. **TransportationTypes_API_Documentation.md** - Complete API documentation with examples

## API Endpoints Summary

### Authentication Endpoints
- `POST /api/auth/register` - Register new rider account
- `POST /api/auth/register-admin` - Register admin account (Admin only)
- `POST /api/auth/register-driver` - Register driver account (Admin only)
- `POST /api/auth/login` - Login and receive JWT token

### Transportation Types Endpoints (NEW)
- `POST /api/transportationtypes` - Create type (Admin)
- `GET /api/transportationtypes` - List all active types
- `GET /api/transportationtypes?includeDeleted=true` - List with deleted (Admin)
- `GET /api/transportationtypes/{id}` - Get by ID
- `PUT /api/transportationtypes/{id}` - Update type (Admin)
- `DELETE /api/transportationtypes/{id}` - Soft delete (Admin)

### Health Check
- `GET /api/ping` - API health check

## How to Test the Transportation Types API

1. **Start the API server** (if not running):
   ```bash
   cd API
   dotnet run
   ```

2. **Register and login** to get an admin token:
   ```bash
   # Register a user
   curl -X POST http://localhost:5270/api/auth/register \
     -H "Content-Type: application/json" \
     -d '{"email": "admin@test.com", "password": "Admin123!", "displayName": "Test Admin"}'
   
   # Update to admin role in database
   mysql -u root qlipickup -e "UPDATE Users SET Role = 'Admin' WHERE Email = 'admin@test.com';"
   
   # Login to get admin token
   curl -X POST http://localhost:5270/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email": "admin@test.com", "password": "Admin123!"}'
   ```

3. **Test Transportation Types endpoints** (use token from login):
   ```bash
   # Create a transportation type
   curl -X POST http://localhost:5270/api/transportationtypes \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{"label": "Medical", "description": "Medical appointments", "sortOrder": 1, "isActive": true}'
   
   # List all types
   curl -X GET http://localhost:5270/api/transportationtypes \
     -H "Authorization: Bearer YOUR_TOKEN"
   ```

4. **Or use the provided test file**:
   - Open `API/TransportationTypes.http` in VS Code
   - Install "REST Client" extension
   - Update the `@token` variable with your JWT token
   - Click "Send Request" above each test

## Branches

 - Blake-demo (Notifications API Endpoints)
 - Gavin-demo (Trips and Trip Action API Endpoints)
 - Gerald-demo (Hours/Schedule and Reports API Endpoints)
 - Angel-demo (Riders, Drivers, and Vehicles API Endpoints)
 - Matthew-demo (GPS Tracking API Endpoint)