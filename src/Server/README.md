# AudiSense Server API

This is the server-side implementation of the AudiSense application, providing Web API endpoints for hearing test management.

## Architecture

The server follows Clean Architecture principles with the following layers:

### Domain Layer (`AudiSense.Domain`)
- Contains core business entities
- No dependencies on external frameworks
- Location: `src/Shared/AudiSense.Domain/`

### Contracts Layer (`AudiSense.Contracts`)
- Contains DTOs and service interfaces
- Shared between client and server
- Location: `src/Shared/AudiSense.Contracts/`

### Infrastructure Layer (`AudiSense.Infrastructure`)
- Contains data access implementations
- Uses Entity Framework Core with InMemory database
- Repository pattern implementation
- Location: `src/Server/AudiSense.Infrastructure/`

### Application Layer (`AudiSense.Application`)
- Contains business logic services
- Implements service interfaces from Contracts layer
- Location: `src/Server/AudiSense.Application/`

### API Layer (`AudiSense.Api`)
- Contains REST API controllers
- Exposes HTTP endpoints
- Swagger documentation enabled
- Location: `src/Server/AudiSense.Api/`

## Features

### HearingTest Entity
The API provides full CRUD operations for hearing tests:

- **Properties:**
  - `Id`: Unique identifier
  - `TesterName`: Name of the person conducting the test (required, max 100 chars)
  - `DateConducted`: Date when the test was performed (required)
  - `Result`: Test results description (required, max 500 chars)

### API Endpoints

#### GET `/api/hearingtests`
Returns all hearing tests in the system.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "testerName": "Dr. Smith",
    "dateConducted": "2024-01-20T10:30:00Z",
    "result": "Normal hearing range"
  }
]
```

#### GET `/api/hearingtests/{id}`
Returns a specific hearing test by ID.

**Response:** `200 OK` or `404 Not Found`

#### POST `/api/hearingtests`
Creates a new hearing test.

**Request Body:**
```json
{
  "testerName": "Dr. Johnson",
  "dateConducted": "2024-01-21T14:00:00Z",
  "result": "Mild hearing loss detected"
}
```

**Response:** `201 Created`

#### PUT `/api/hearingtests/{id}`
Updates an existing hearing test.

**Request Body:** Same as POST
**Response:** `200 OK` or `404 Not Found`

#### DELETE `/api/hearingtests/{id}`
Deletes a hearing test.

**Response:** `204 No Content` or `404 Not Found`

## Database

- **Provider:** Entity Framework Core InMemory Database
- **Database Name:** `AudiSenseDb`
- **Seeded Data:** Two sample hearing tests for testing

## Running the API

1. **Build the solution:**
   ```bash
   dotnet build src/AudiSense.sln
   ```

2. **Run the API:**
   ```bash
   dotnet run --project src/Server/AudiSense.Api
   ```

3. **Access Swagger UI:**
   - Development: `https://localhost:7001/swagger` or `http://localhost:5001/swagger`

## Client Integration

The WPF client can consume this API using the `IDataService` interface:

```csharp
// Get all hearing tests
var tests = await dataService.GetAsync<IEnumerable<HearingTestResponse>>("api/hearingtests");

// Create a new hearing test
var request = new HearingTestRequest
{
    TesterName = "Dr. Example",
    DateConducted = DateTime.Now,
    Result = "Test completed successfully"
};
var created = await dataService.PostAsync<HearingTestRequest, HearingTestResponse>("api/hearingtests", request);

// Update a hearing test
await dataService.UpdateAsync("api/hearingtests/1", request);

// Delete a hearing test
await dataService.DeleteAsync("api/hearingtests/1");
```

## CORS Configuration

The API is configured to allow all origins, methods, and headers for development purposes. For production deployment, configure specific allowed origins.

## Validation

Request DTOs include validation attributes:
- Required fields validation
- String length validation
- Custom error messages

## Logging

The API uses built-in .NET logging:
- Information level for normal operations
- Warning level for failed requests
- Error level for exceptions
