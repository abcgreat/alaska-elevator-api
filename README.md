# Elevator API (Alaska Airlines – Senior Consultant Assignment)

Minimal, transparent Elevator Service API intended to unblock dependent teams quickly.  
Built with **C# / ASP.NET Core (.NET 8 Minimal API)** and developed using **TDD**.

---

## Quick Start

### Run the API
```bash
dotnet run --project ElevatorApi.Api
```

API listens on:
- `http://localhost:8080`
- In Codespaces: forwarded port **8080**  
  - In the **PORTS** panel, set port `8080` to **Public** so reviewers can access Swagger and the API from a browser.

### Run tests
```bash
dotnet test
```

### Swagger
- Local: `http://localhost:8080/swagger`
- Codespaces: `https://<your-codespace>-8080.app.github.dev/swagger`

---

## What’s Implemented

### Scenario #1 — Request an elevator to the caller’s floor (Pickup)
**POST** `/api/elevator/request`

Body:
```json
{ "floor": 5 }
```

Response — `202 Accepted`:
```json
{
  "requestedFloor": 5,
  "status": "queued"
}
```

Error — `400 Bad Request`:
```json
{ "error": "Floor must be >= 1" }
```

---

### Scenario #2 — Request a destination floor
**POST** `/api/elevator/destination`

Body:
```json
{ "floor": 10 }
```

Response — `202 Accepted`:
```json
{
  "destinationFloor": 10,
  "status": "queued"
}
```

Error — `400 Bad Request`:
```json
{ "error": "Floor must be >= 1" }
```

---
### Scenario #3 — Get floors current passengers are servicing
**GET** `/api/elevator/stops`

Response — `200 OK`:
```json
{
  "stops": [3, 10]
}
```

Notes:

- Floors are unique and sorted ascending.
- Returns `{ "stops": [] }` if no destinations are queued yet.

---

### Scenario #4 — Get next stop
**GET** `/api/elevator/next`

Response — `200 OK`:
```json
{
  "nextStop": 3
}
```

Notes:
- Returns the **smallest queued stop** (peek behavior).
- Returns `{ "nextStop": null }` if no stops are currently queued.
- Does **not** remove the stop from the queue; use `/api/elevator/stops` to inspect all pending stops.

---
## Assumptions

To keep the integration surface small and deterministic for this exercise:

1. **Single elevator car** is modeled initially.  
   The API is designed to extend to multiple cars later.

2. **Floors are positive integers.**  
   Requests with `floor < 1` return `400 Bad Request`.

3. **Scheduling/dispatch logic is intentionally minimal.**  
   Priority is stable API contracts and predictable behavior.

4. **In-memory state only** (no persistence) for contract-first iteration.

5. `/api/elevator/next` uses **peek semantics** and does not mutate the in-memory state.

---

## TDD Notes

This project follows red → green → refactor:

1. Add failing contract test in `ElevatorApi.Tests`
2. Implement smallest endpoint to satisfy the test
3. Refactor while keeping tests green

Commit history reflects these TDD slices (tests-first, then minimal implementation).

---

## Code that connects to the API and processes the output

This repo includes code that acts as a client of the Elevator API and processes its responses:

1. **Integration tests (`ElevatorApi.Tests`)**

   - Tests use `WebApplicationFactory<Program>` to spin up the API in memory.
   - They send real HTTP requests to endpoints such as  
     `/api/elevator/request`, `/api/elevator/destination`, `/api/elevator/stops`,
     and `/api/elevator/next`.
   - Responses are read as JSON and deserialized into DTOs, and the tests assert on:
     - HTTP status codes (`202 Accepted`, `200 OK`, `400 Bad Request`)
     - Response body shape (properties like `requestedFloor`, `destinationFloor`, `stops`, `nextStop`)
     - Business behavior (sorted unique stops, peek semantics for `nextStop`).

2. **Manual client (`ElevatorApi.Api.http`)**

   - The `ElevatorApi.Api.http` file contains ready-to-run HTTP requests that call the API
     from VS Code’s REST client.
   - These requests exercise all four scenarios and show the raw JSON returned by the API.

Together, these act as “code that connects to the API and processes the output,” both in
an automated way (tests) and an interactive/manual way (`.http` file).

---

## Future Work / Extensions

The current implementation is intentionally minimal and contract-first to unblock dependent teams. In a real system, I would explore:

1. **Multiple elevator cars**
   - Model multiple cars with IDs and independent state.
   - Extend contracts to include `carId` and per-car stops.
   - Introduce a dispatcher that assigns requests to cars.

2. **Direction- and load-aware scheduling**
   - Prefer serving requests in the current travel direction.
   - Consider capacity and load (e.g., max passengers) per car.
   - Support different scheduling strategies (e.g., nearest-car, sector-based).

3. **State persistence**
   - Replace in-memory `ElevatorState` with a backing store (SQL/NoSQL).
   - Persist requests, stops, and elevator positions across restarts.
   - Add optimistic concurrency / versioning for updates.

4. **Event-driven architecture**
   - Emit events like `ElevatorRequested`, `StopServed`, `CarMoved`.
   - Use a message bus (e.g., Azure Service Bus) to decouple UI, scheduler, and telemetry.
   - Support replay and auditing for incident investigation.

5. **Richer domain model**
   - Track current floor, direction, and door state.
   - Distinguish between *pickup* and *dropoff* stops internally.
   - Add validation for building constraints (min/max floor per elevator).

6. **Operational hardening**
   - Add structured logging, correlation IDs, and metrics per endpoint.
   - Define rate limits and input validation rules.
   - Extend test suite with property-based tests and error-path coverage.
