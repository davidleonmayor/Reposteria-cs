public sealed class CategoryRepository : CrudSqlRepository<Category, int>, ICategoryRepository
{
    public CategoryRepository(IDapperHelper db) : base(db, new CategoryMap())
    {
    }

    private sealed class CategoryMap : ISqlMap<Category, int>
    {
        public string Table => "Category";
        public string KeyColumn => "Id";

        public string SelectColumns => "Id, Name, Description";
        public string InsertColumns => "Name, Description";
        public string InsertValues => "@Name, @Description";
        public string UpdateSetClause => "Name=@Name, Description=@Description";

        public object ToKeyParam(int id) => new { Id = id };
        public object ToInsertParams(Category entity) => entity;
        public object ToUpdateParams(Category entity) => entity;
    }
}
