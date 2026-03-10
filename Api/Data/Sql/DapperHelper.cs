using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;

public sealed class DapperHelper : IDapperHelper
{
    private readonly IDbConnectionFactory _factory;

    public DapperHelper(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx is not null)
            return await tx.Connection!.QueryAsync<T>(sql, param, tx);

        await using var conn = await _factory.OpenAsync();
        return await conn.QueryAsync<T>(sql, param);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx is not null)
            return await tx.Connection!.QueryFirstOrDefaultAsync<T>(sql, param, tx);

        await using var conn = await _factory.OpenAsync();
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        string splitOn = "Id",
        object? param = null,
        IDbTransaction? tx = null)
    {
        if (tx is not null)
            return await tx.Connection!.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, tx, splitOn: splitOn);

        await using var conn = await _factory.OpenAsync();
        return await conn.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, splitOn: splitOn);
    }

    public async Task<TReturn?> QueryFirstOrDefaultAsync<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        string splitOn = "Id",
        object? param = null,
        IDbTransaction? tx = null)
    {
        var rows = await QueryAsync(sql, map, splitOn, param, tx);
        return rows.FirstOrDefault();
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx is not null)
            return await tx.Connection!.ExecuteAsync(sql, param, tx);

        await using var conn = await _factory.OpenAsync();
        return await conn.ExecuteAsync(sql, param);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx is not null)
        {
            var result = await tx.Connection!.ExecuteScalarAsync<T?>(sql, param, tx);
            if (result is null)
                throw new InvalidOperationException("ExecuteScalar returned null.");
            return result;
        }

        await using var conn = await _factory.OpenAsync();
        var result2 = await conn.ExecuteScalarAsync<T?>(sql, param);
        if (result2 is null)
            throw new InvalidOperationException("ExecuteScalar returned null.");
        return result2;
    }

    public async Task<T> InTransactionAsync<T>(Func<DbConnection, IDbTransaction, Task<T>> work, CancellationToken ct = default)
    {
        await using var conn = await _factory.OpenAsync(ct);
        using var tx = conn.BeginTransaction();
        try
        {
            var result = await work(conn, tx);
            tx.Commit();
            return result;
        }
        catch
        {
            try { tx.Rollback(); } catch { /* ignore rollback failures */ }
            throw;
        }
    }
}
