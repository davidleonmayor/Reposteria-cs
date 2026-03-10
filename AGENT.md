# Reposteria-cs (Project Context)

This repo contains a single .NET Web API project used for a small bakery/sales domain.

## What it is
- Project: `Api/` (ASP.NET Core, `net10.0`)
- ORM/DB: EF Core + SQLite (`Api/app.db`)
- Domains: People (Person/PersonType), Catalog (Category/Product), Sales (Sale + participants + details)

## Run locally
- Build: `dotnet build`
- Run API: `dotnet run --project Api/Api.csproj`
- Dev UI: Swagger at `/swagger`

## Auth and authorization (dev/demo)
- Login: `POST /api/Auth/login` returns a JWT.
- Demo users live in `Api/appsettings.json` under `Auth:Users`.
- Authorization policies (see `Api/Program.cs`):
  - `CatalogWrite`: roles `Admin`, `Manager`
  - `PeopleWrite`: role `Admin`
  - `SalesWrite`: roles `Admin`, `Seller`
- Most `GET` endpoints are `[AllowAnonymous]`; write endpoints require policies.

## API conventions
- Requests use DTOs in `Api/Dtos/` (avoid binding EF entities directly).
- Input strings are normalized via `StringNormalization.Clean`.
- Global error handling: `Api/Middleware/ExceptionHandlingMiddleware.cs` returns RFC7807 `ProblemDetails`.
- Schema changes go through migrations in `Api/Migrations/`.

## Quick verification
- End-to-end curl flow lives in `Api/curl-flow.txt`.
