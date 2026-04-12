# API Documentation

## Base URL
`http://localhost:5270/api`

## Authentication
All endpoints except register and login require a JWT token.
Include the token in the Authorization header: `Authorization: Bearer <token>`

---

## Auth Endpoints

### POST /api/auth/register
Register a new rider account. No authentication required.
```json
{
  "email": "user@example.com",
  "password": "password123",
  "displayName": "John Smith"
}
```
Returns: user info + JWT token

### POST /api/auth/login
Login with email and password. No authentication required.
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```
Returns: user info + JWT token

### POST /api/auth/register-admin
Create an admin account. Admin only.
Same body as register.

### POST /api/auth/register-driver
Create a driver account. Admin only.
Same body as register.

---

## Rider Endpoints

### GET /api/rider
List all active riders. Any authenticated user.
Optional query: `?includeInactive=true` (admin only)

### GET /api/rider/{id}
Get a specific rider. Any authenticated user.

### POST /api/rider
Create a rider profile. Admin only.
```json
{
  "userId": 1,
  "firstName": "Dorothy",
  "lastName": "Hargrove",
  "phone": "402-555-1234",
  "roomNumber": "214B",
  "mobilityNotes": "Uses wheelchair",
  "emergencyContactName": "James Hargrove",
  "emergencyContactPhone": "402-555-5678"
}
```

### PUT /api/rider/{id}
Update a rider. Admin only. Same fields as create minus userId.

### DELETE /api/rider/{id}
Soft delete a rider (sets IsActive to false). Admin only.

---

## Driver Endpoints

### GET /api/driver
List all active drivers. Any authenticated user.
Optional query: `?includeInactive=true` (admin only)

### GET /api/driver/{id}
Get a specific driver. Any authenticated user.

### POST /api/driver
Create a driver profile. Admin only.
```json
{
  "userId": 2,
  "firstName": "Carlos",
  "lastName": "Mendoza",
  "phone": "402-555-9876",
  "licenseNumber": "NE-12345678",
  "licenseExpiry": "2027-09-15"
}
```

### PUT /api/driver/{id}
Update a driver. Admin only. Same fields as create minus userId.

### DELETE /api/driver/{id}
Soft delete a driver. Admin only.

---

## Vehicle Endpoints

### GET /api/vehicle
List all active vehicles. Any authenticated user.
Optional query: `?includeInactive=true` (admin only)

### GET /api/vehicle/{id}
Get a specific vehicle. Any authenticated user.

### POST /api/vehicle
Create a vehicle. Admin only.
```json
{
  "displayName": "Van #3",
  "make": "Ford",
  "model": "Transit 350 HD",
  "year": 2022,
  "licensePlate": "QLI-7734",
  "vin": "1FTBW3XM6NKA12345",
  "seatCapacity": 8,
  "wheelchairCapacity": 2,
  "odometer": 24318
}
```

### PUT /api/vehicle/{id}
Update a vehicle. Admin only. Same fields as create.

### DELETE /api/vehicle/{id}
Soft delete a vehicle. Admin only.

---

## Transportation Type Endpoints

### GET /api/transportationtypes
List all transportation types. Any authenticated user.
Optional query: `?includeDeleted=true` (admin only)

### GET /api/transportationtypes/{id}
Get a specific type. Any authenticated user.

### POST /api/transportationtypes
Create a type. Admin only.
```json
{
  "label": "Medical",
  "description": "Medical appointments and procedures",
  "sortOrder": 1
}
```

### PUT /api/transportationtypes/{id}
Update a type. Admin only.

### DELETE /api/transportationtypes/{id}
Soft delete a type. Admin only. Blocked if active trips use this type.

---

## Trip Endpoints

### GET /api/trips
List trips with optional filters. Any authenticated user.
Query parameters:
- `?status=Pending` (Pending, Approved, Denied, Scheduled, InProgress, Completed, Cancelled, NoShow)
- `?riderId=1`
- `?driverId=1`
- `?transportationTypeId=1`
- `?date=2026-04-15`

### GET /api/trips/{id}
Get a specific trip. Any authenticated user.

### POST /api/trips
Create a trip request. Any authenticated user.
```json
{
  "riderId": 1,
  "transportationTypeId": 1,
  "pickupAddress": "123 Main St, Lincoln, NE",
  "destinationAddress": "456 Oak Ave, Lincoln, NE",
  "scheduledPickupTime": "2026-04-15T09:30:00",
  "passengerCount": 1,
  "requiresWheelchair": true,
  "notes": "Morning appointment"
}
```
New trips start with status "Pending".

### PUT /api/trips/{id}
Update a trip. Admin only. Only editable before InProgress.

### DELETE /api/trips/{id}
Soft delete a trip. Admin only.

---

## Trip Status Actions

Each action enforces the state machine — only valid transitions are allowed.
Every status change is logged in the audit trail.

Valid transitions:
- Pending → Approved, Denied, Cancelled
- Approved → Scheduled, Cancelled
- Scheduled → InProgress, Cancelled
- InProgress → Completed, NoShow

### POST /api/trips/{id}/approve
Approve a pending trip. Admin only.

### POST /api/trips/{id}/deny
Deny a pending trip. Admin only. Reason required.
```json
{
  "reason": "No vehicles available for this time slot"
}
```

### POST /api/trips/{id}/assign
Assign a driver and vehicle to an approved trip. Admin only.
```json
{
  "driverId": 1,
  "vehicleId": 1
}
```
Moves trip to Scheduled status.

### POST /api/trips/{id}/start
Driver starts the trip. Driver only. Records actual pickup time.

### POST /api/trips/{id}/complete
Driver completes the trip. Driver only. Records dropoff time and distance.
```json
{
  "distanceMiles": 7.4
}
```

### POST /api/trips/{id}/noshow
Driver marks rider as absent. Driver only.

### POST /api/trips/{id}/cancel
Cancel a trip. Any authenticated user. Optional reason.
```json
{
  "reason": "Rider no longer needs transportation"
}
```

---

## Trip Status History

### GET /api/tripstatushistory/trip/{tripId}
Get the full audit trail for a trip. Shows every status change with who made it, when, and why.
