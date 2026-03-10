# Auth (JWT)

## Login endpoint
- `POST /api/Auth/login`
- Body: `{ "username": "...", "password": "..." }`
- Response: `{ token, expiresAt }`

## User source (dev/demo)
- Users are configured in `Api/appsettings.json` under `Auth:Users`.
- Roles from config are emitted as `ClaimTypes.Role` in the JWT.

## Authorization model
- Controllers use `[Authorize(Policy = "...")]` for write operations.
- Most `GET` endpoints are `[AllowAnonymous]`.
