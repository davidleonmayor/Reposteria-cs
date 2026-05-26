# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Monorepo structure

Two independent sub-projects live side by side:

| Path | Stack |
|------|-------|
| `Api/` | ASP.NET Core (`net10.0`) + EF Core + SQLite |
| `Api.Tests/` | xUnit integration tests (uses `WebApplicationFactory`) |
| `web/` | Next.js (App Router) + React 19 + Tailwind v4 + Zustand |

Each sub-project has its own CLAUDE.md / AGENT.md with deeper notes — read them when working inside that subtree. `web/CLAUDE.md` has detailed frontend guidance.

## Commands

### API (run from repo root or `Api/`)
```bash
dotnet build                            # build solution
dotnet run --project Api/Api.csproj    # starts on https://localhost:5001, Swagger at /swagger
dotnet test                             # run all xUnit tests
dotnet test --filter "FullyQualifiedName~CategoryEndpointTests"  # run a single test class
dotnet ef migrations add <Name> --project Api  # add EF migration
dotnet ef database update --project Api        # apply migrations to app.db
```

### Frontend (run from `web/`)
```bash
pnpm dev      # Next.js dev server — http://localhost:3000
pnpm build    # production build (also type-checks)
pnpm lint     # ESLint (flat config)
```

No `pnpm test` — there is no test runner configured for the frontend.

## API architecture

The API follows a **vertical-slice / modular architecture** under `Api/Core/Modules/`. Each module is fully self-contained:

```
Api/Core/Modules/<Module>/
  Domain/Entities/          # EF entity (plain C# class)
  Application/Interfaces/   # repository interface
  Application/UseCases/     # one class per operation (GetAll, GetById, Create, Update, Delete)
  Application/DTOs/         # request/response shapes
  Infrastructure/Persistence/   # EF repository implementation
  Infrastructure/Presentation/  # endpoint mapping (Minimal API) or Controller
```

Modules: `Auth`, `Categories`, `Products`, `PersonTypes`, `Persons`, `Sales`, `SaleDetails`, `SaleParticipants`.

**Key wiring rules:**
- All use cases and repositories are registered in `Api/Program.cs` as `AddScoped`.
- `AppDbContext` lives in `Api/Data/` and is the single EF context for all modules.
- Minimal API endpoints are mapped via extension methods (`MapXxxEndpoints()`) called in `Program.cs`; some modules still use controllers (`MapControllers()`).
- Input strings are cleaned via `StringNormalization.Clean` (in `Api/Utils/`) before persistence.
- All unhandled exceptions are caught by `Api/Middleware/ExceptionHandlingMiddleware.cs`, which returns RFC 7807 `ProblemDetails`.

## Auth

- `POST /api/Auth/login` — returns a JWT. Demo credentials are hardcoded in `appsettings.json` under `Auth:Users` (admin / seller).
- Authorization policies in `Program.cs`: `CatalogWrite` (Admin, Manager), `PeopleWrite` (Admin), `SalesWrite` (Admin, Seller).
- Most `GET` endpoints are `[AllowAnonymous]`.

## Testing patterns

Tests in `Api.Tests/` are functional integration tests using `WebApplicationFactory<Program>`.

- `ApiWebFactory` (in `Fixtures/`) replaces the real SQLite DB with an in-memory SQLite connection and swaps JWT Bearer for `TestAuthHandler` (auto-authenticates every request).
- Call `ResetDatabaseAsync()` at the start of each test to get a clean DB.
- Use the `Seed*Async()` helpers on `ApiWebFactory` to insert prerequisite data (they must be called in dependency order: PersonType → Person → Category → Product → Sale).
- Test files mirror the module structure: `Api.Tests/Modules/<Module>/<Module>EndpointTests.cs`.

## Domain model (quick reference)

See `Api/README.md` for the full ER diagram. Key relationships:
- `Person` → classified by `PersonType`
- `Product` → belongs to `Category`
- `Sale` → has many `SaleDetail` (Product + quantity + unit price) and many `SaleParticipant` (Person + role)
