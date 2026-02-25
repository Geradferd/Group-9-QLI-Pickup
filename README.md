# Group-9-QLI-Pickup

## What is this application?

QLIFT is a pickup coordination system for QLI (Quality Living, Inc.) that helps manage transportation scheduling between clients and drivers.

## How to use

### Development Mode

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

## Release Notes

### Current Submission - Project Setup

**What's working:**
- ASP.NET Core API with controller support
- React + TypeScript frontend
- API-frontend communication (CORS configured)
- Swagger API documentation
- Health check endpoint (`/api/ping`)
- Development environment with hot reload
- Production build and deployment setup

**Note:** This submission focuses on establishing the project infrastructure and ensuring the frontend and backend can communicate properly. Core features for pickup coordination will be implemented in future milestones.

## Branches

Currently all code is in the **main** branch.
