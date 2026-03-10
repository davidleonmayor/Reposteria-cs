public sealed class PersonTypeRepository : CrudSqlRepository<PersonType, int>, IPersonTypeRepository
{
    public PersonTypeRepository(IDapperHelper db) : base(db, new PersonTypeMap())
    {
    }

    private sealed class PersonTypeMap : ISqlMap<PersonType, int>
    {
        public string Table => "PersonType";
        public string KeyColumn => "Id";

        public string SelectColumns => "Id, Name, Description";
        public string InsertColumns => "Name, Description";
        public string InsertValues => "@Name, @Description";
        public string UpdateSetClause => "Name=@Name, Description=@Description";

        public object ToKeyParam(int id) => new { Id = id };
        public object ToInsertParams(PersonType entity) => entity;
        public object ToUpdateParams(PersonType entity) => entity;
    }
}
