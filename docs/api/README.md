# Group-9-QLI-Pickup

## What is this application?
QLIFT is a pickup coordination system for QLI (Quality Living, Inc.) that helps manage transportation scheduling between clients and drivers.

## Prerequisites
Install these before running the project:
- **.NET 10 SDK** — https://dotnet.microsoft.com/download/dotnet/10.0
- **Node.js** (LTS) — https://nodejs.org
- **MySQL 8.4** — https://dev.mysql.com/downloads/mysql/
- **EF Core Tools** — run: `dotnet tool install --global dotnet-ef`

## Getting Started
1. Clone the repo:
   ```bash
   git clone https://github.com/Geradferd/Group-9-QLI-Pickup.git
   cd Group-9-QLI-Pickup
   ```
2. Log into MySQL and create the database:
   ```bash
   mysql -u root -p
   ```
   ```sql
   CREATE DATABASE qlipickup;
   ```
   Type `exit` to leave MySQL.

3. Open `API/appsettings.json` and replace the password with your local MySQL root password:
   ```
   "Password=YOUR_PASSWORD_HERE;"
   ```

4. Run the backend:
   ```bash
   cd API
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

5. In a separate terminal, run the frontend:
   ```bash
   cd Client
   npm install
   npm run dev
   ```

6. Test the API by visiting `http://localhost:5270/swagger`

## What's Been Built

### Authentication 
- User registration and login with JWT token authentication
- Password hashing with BCrypt
- Role-based authorization (Admin, Driver, Rider)
- Only admins can create driver and admin accounts

### CRUD Endpoints 
- Riders — list, get by ID, create, update, soft delete
- Drivers — list, get by ID, create, update, soft delete
- Vehicles — list, get by ID, create, update, soft delete
- Transportation Types — list, get by ID, create, update, soft delete

### Trip System 
- Trip creation with pickup/destination, scheduling, passenger count, wheelchair requirement
- Trip filtering by status, rider, driver, transportation type, and date
- Full trip status lifecycle: Pending → Approved → Scheduled → InProgress → Completed
- Status actions: approve, deny (with reason), assign driver/vehicle, start, complete, no-show, cancel
- State machine enforcement — only valid transitions are allowed
- Audit logging — every status change is recorded with who, when, and why
- Trip editing is locked once a trip is in progress

### Security
- JWT tokens expire after 60 minutes
- DTOs prevent sensitive data like password hashes from leaving the server
- Soft delete on all entities — no data is permanently destroyed (NFR-05)
- Role-based access on every endpoint (NFR-03)


