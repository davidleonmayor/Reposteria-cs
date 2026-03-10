public abstract class CrudSqlRepository<TEntity, TKey> : ICrudRepository<TEntity, TKey>
{
    protected readonly IDapperHelper Db;
    protected readonly ISqlMap<TEntity, TKey> Map;

    protected CrudSqlRepository(IDapperHelper db, ISqlMap<TEntity, TKey> map)
    {
        Db = db;
        Map = map;
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
        => Db.QueryAsync<TEntity>($"SELECT {Map.SelectColumns} FROM {Map.Table};");

    public Task<TEntity?> GetByIdAsync(TKey id)
        => Db.QueryFirstOrDefaultAsync<TEntity>(
            $"SELECT {Map.SelectColumns} FROM {Map.Table} WHERE {Map.KeyColumn} = @{Map.KeyColumn};",
            Map.ToKeyParam(id));

    public async Task<TKey> AddAsync(TEntity entity)
    {
        // SQLite: last_insert_rowid() is connection-scoped.
        var sql = $@"INSERT INTO {Map.Table} ({Map.InsertColumns})
VALUES ({Map.InsertValues});
SELECT last_insert_rowid();";

        var newId = await Db.ExecuteScalarAsync<long>(sql, Map.ToInsertParams(entity));
        return (TKey)Convert.ChangeType(newId, typeof(TKey));
    }

    public Task<int> UpdateAsync(TEntity entity)
        => Db.ExecuteAsync(
            $"UPDATE {Map.Table} SET {Map.UpdateSetClause} WHERE {Map.KeyColumn} = @{Map.KeyColumn};",
            Map.ToUpdateParams(entity));

    public Task<int> DeleteAsync(TKey id)
        => Db.ExecuteAsync(
            $"DELETE FROM {Map.Table} WHERE {Map.KeyColumn} = @{Map.KeyColumn};",
            Map.ToKeyParam(id));
}
