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
  (set Visibility to **Public** before submission so reviewers can access Swagger)

### Run tests
```bash
dotnet test
```

### Swagger
- Local: `http://localhost:8080/swagger`
- Codespaces: `https://<your-codespace>-8080.app.github.dev/swagger`

> `/weatherforecast` is a temporary template smoke endpoint and will be removed once elevator endpoints are complete.

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
