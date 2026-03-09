# Overview

This repository currently contains a single API project: `Api/`.

## Tech
- .NET: ASP.NET Core (`net10.0`)
- Data: EF Core + SQLite
- Auth: JWT Bearer + role-based policies
- Dev docs: Swagger UI (enabled in Development)

## High-level structure
- `Api/Controllers/`: HTTP endpoints
- `Api/Models/`: EF Core entities (domain)
- `Api/Dtos/`: request DTOs + validation (DataAnnotations)
- `Api/Data/AppDbContext.cs`: EF Core mappings/relationships
- `Api/Middleware/`: global exception handling
- `Api/Migrations/`: database migrations

## Domain at a glance
See `Api/README.md` for the ER diagram.
