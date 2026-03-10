# AuthController

Path: `Api/Controllers/AuthController.cs`

## Routes
- `POST /api/Auth/login` (anonymous)

## Behavior
- Validates credentials against `Auth:Users` in configuration.
- Issues a JWT with role claims (`ClaimTypes.Role`).
