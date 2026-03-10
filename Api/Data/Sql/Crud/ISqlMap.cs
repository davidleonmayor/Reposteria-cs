public interface ISqlMap<TEntity, TKey>
{
    string Table { get; }
    string KeyColumn { get; }

    string SelectColumns { get; }
    string InsertColumns { get; }
    string InsertValues { get; }
    string UpdateSetClause { get; }

    object ToKeyParam(TKey id);
    object ToInsertParams(TEntity entity);
    object ToUpdateParams(TEntity entity);
}
