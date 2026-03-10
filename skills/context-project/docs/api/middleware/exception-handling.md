# ExceptionHandlingMiddleware

Path: `Api/Middleware/ExceptionHandlingMiddleware.cs`

## Purpose
- Convert unhandled exceptions into RFC7807 `ProblemDetails` responses.
- Keep a `traceId` in the response to correlate logs.

## Mapping
- `BadHttpRequestException`, `ArgumentException`, `InvalidOperationException` -> 400.
- `DbUpdateException` with SQLite constraints:
  - UNIQUE constraint -> 409
  - FOREIGN KEY constraint -> 400
- Fallback -> 500.
