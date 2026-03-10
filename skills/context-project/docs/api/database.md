# Database (SQLite + EF Core)

## Storage
- SQLite file: `Api/app.db`
- Connection string: `Api/appsettings.json` -> `ConnectionStrings:DefaultConnection`.

## Migrations
- Migrations are in `Api/Migrations/`.
- Typical commands:

```bash
dotnet ef migrations add <Name> --project Api/Api.csproj
dotnet ef database update --project Api/Api.csproj
```

Notes
- This repo already includes a local SQLite file; be careful not to rely on its contents as a source of truth.
