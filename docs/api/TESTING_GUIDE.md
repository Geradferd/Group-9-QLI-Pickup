# Testing Guide

## Setup
1. Make sure MySQL is running and the `qlipickup` database exists
2. Update the password in `API/appsettings.json` to your local MySQL password
3. Run migrations: `cd API && dotnet ef database update`
4. Start the API: `dotnet run`
5. Open a separate terminal for running test commands

## Step 1: Register a User
```bash
curl -s -X POST http://localhost:5270/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"password123","displayName":"Test Admin"}'
```
You should get back a JSON response with an id, email, displayName, role, and token.

## Step 2: Promote to Admin
Since there are no admins yet, manually update the role in MySQL:
```bash
mysql -u root -p -e "USE qlipickup; UPDATE Users SET Role = 'Admin' WHERE Email = 'admin@test.com';"
```

## Step 3: Login as Admin
```bash
curl -s -X POST http://localhost:5270/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"password123"}'
```
Copy the token from the response. Save it for the next steps:
```bash
TOKEN="paste_your_token_here"
```

## Step 4: Create a Transportation Type
```bash
curl -s -X POST http://localhost:5270/api/transportationtypes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"label":"Medical","description":"Medical appointments","sortOrder":1}'
```

## Step 5: Create a Rider
First register a rider user account:
```bash
curl -s -X POST http://localhost:5270/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"rider@test.com","password":"password123","displayName":"Test Rider"}'
```
Note the user ID from the response, then create the rider profile (replace userId with the actual ID):
```bash
curl -s -X POST http://localhost:5270/api/rider \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"userId":2,"firstName":"John","lastName":"Smith","phone":"402-555-1234","roomNumber":"204","mobilityNotes":"Uses wheelchair","emergencyContactName":"Jane Smith","emergencyContactPhone":"402-555-5678"}'
```

## Step 6: Create a Driver
Register a driver account (admin endpoint creates user with Driver role):
```bash
curl -s -X POST http://localhost:5270/api/auth/register-driver \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"email":"driver@test.com","password":"password123","displayName":"Test Driver"}'
```
Note the user ID, then create the driver profile:
```bash
curl -s -X POST http://localhost:5270/api/driver \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"userId":3,"firstName":"Jane","lastName":"Doe","phone":"402-555-9876","licenseNumber":"NE-12345678","licenseExpiry":"2027-09-15"}'
```

## Step 7: Create a Vehicle
```bash
curl -s -X POST http://localhost:5270/api/vehicle \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"displayName":"Van #1","make":"Toyota","model":"Sienna","year":2023,"licensePlate":"ABC-1234","seatCapacity":6,"wheelchairCapacity":1}'
```

## Step 8: Create a Trip
```bash
curl -s -X POST http://localhost:5270/api/trips \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"riderId":1,"transportationTypeId":1,"pickupAddress":"123 Main St","destinationAddress":"456 Oak Ave","scheduledPickupTime":"2026-04-15T09:00:00","passengerCount":1,"requiresWheelchair":true,"notes":"Morning appointment"}'
```
Trip should be created with status "Pending".

## Step 9: Approve the Trip
```bash
curl -s -X POST http://localhost:5270/api/trips/1/approve \
  -H "Authorization: Bearer $TOKEN"
```
Status should change to "Approved".

## Step 10: Assign Driver and Vehicle
```bash
curl -s -X POST http://localhost:5270/api/trips/1/assign \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"driverId":1,"vehicleId":1}'
```
Status should change to "Scheduled".

## Step 11: Login as Driver
```bash
curl -s -X POST http://localhost:5270/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"driver@test.com","password":"password123"}'
```
Save the driver token:
```bash
DRIVER_TOKEN="paste_driver_token_here"
```

## Step 12: Start the Trip
```bash
curl -s -X POST http://localhost:5270/api/trips/1/start \
  -H "Authorization: Bearer $DRIVER_TOKEN"
```
Status should change to "InProgress" and actual pickup time should be stamped.

## Step 13: Complete the Trip
```bash
curl -s -X POST http://localhost:5270/api/trips/1/complete \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $DRIVER_TOKEN" \
  -d '{"distanceMiles":5.2}'
```
Status should change to "Completed" with dropoff time and distance recorded.

## Step 14: Check the Audit Trail
```bash
curl -s -X GET http://localhost:5270/api/tripstatushistory/trip/1 \
  -H "Authorization: Bearer $TOKEN"
```
You should see entries for: Created, Approved, Scheduled (assigned), InProgress (started), Completed.

## Testing Invalid Transitions
Try these to verify the state machine blocks invalid transitions:

Try to approve a completed trip (should fail):
```bash
curl -s -X POST http://localhost:5270/api/trips/1/approve \
  -H "Authorization: Bearer $TOKEN"
```

Try to access admin endpoints as a rider (should get 403):
```bash
curl -s -X POST http://localhost:5270/api/rider \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $DRIVER_TOKEN" \
  -d '{"userId":1,"firstName":"Test","lastName":"Test"}'
```

## Verifying in the Database
You can check the data directly in MySQL:
```bash
mysql -u root -p
USE qlipickup;
SELECT Id, Email, DisplayName, Role, IsActive FROM Users;
SELECT Id, FirstName, LastName, RoomNumber, MobilityNotes FROM Riders;
SELECT Id, FirstName, LastName, LicenseNumber FROM Drivers;
SELECT Id, DisplayName, Make, Model, SeatCapacity FROM Vehicles;
SELECT Id, Status, PickupAddress, DestinationAddress FROM Trips;
SELECT Id, TripId, FromStatus, ToStatus, Reason FROM TripStatusHistories;
```
