using System.Data.Common;
using Microsoft.Data.Sqlite;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _config;

    public SqliteConnectionFactory(IConfiguration config) => _config = config;

    public async Task<DbConnection> OpenAsync(CancellationToken ct = default)
    {
        var cs = _config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Missing connection string: ConnectionStrings:DefaultConnection");

        var conn = new SqliteConnection(cs);
        await conn.OpenAsync(ct);

        // Enforce FK constraints (SQLite default is OFF).
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "PRAGMA foreign_keys = ON;";
            await cmd.ExecuteNonQueryAsync(ct);
        }

        return conn;
    }
}
