# Api/Program.cs

`Api/Program.cs` wires up the API.

## Services
- `AppDbContext` configured with SQLite connection string `DefaultConnection`.
- Controllers via `AddControllers()`.
- Swagger via `AddSwaggerGen()` + Bearer security definition.
- JWT auth via `AddAuthentication().AddJwtBearer()`.
- Authorization policies:
  - `CatalogWrite`: `Admin`, `Manager`
  - `PeopleWrite`: `Admin`
  - `SalesWrite`: `Admin`, `Seller`

## Middleware pipeline
- Swagger/SwaggerUI in Development only.
- `ExceptionHandlingMiddleware` for RFC7807 `ProblemDetails` responses.
- `UseHttpsRedirection()`, then auth (`UseAuthentication()` + `UseAuthorization()`), then `MapControllers()`.
