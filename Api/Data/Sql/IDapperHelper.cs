using System.Data;
using System.Data.Common;

public interface IDapperHelper
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? tx = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? tx = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        string splitOn = "Id",
        object? param = null,
        IDbTransaction? tx = null);

    Task<TReturn?> QueryFirstOrDefaultAsync<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        string splitOn = "Id",
        object? param = null,
        IDbTransaction? tx = null);

    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? tx = null);
    Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? tx = null);

    Task<T> InTransactionAsync<T>(Func<DbConnection, IDbTransaction, Task<T>> work, CancellationToken ct = default);
}
