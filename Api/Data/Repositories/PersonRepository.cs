public sealed class PersonRepository : IPersonRepository
{
    private readonly IDapperHelper _db;

    public PersonRepository(IDapperHelper db) => _db = db;

    private const string PersonColumns =
        "p.Id, p.PersonTypeId, p.Name, p.LastName, p.Phone, p.Email, p.Address, p.RegisterDate, p.Active";

    private const string PersonTypeColumns =
        "pt.Id, pt.Name, pt.Description";

    private const string JoinSql =
        " FROM Person p LEFT JOIN PersonType pt ON p.PersonTypeId = pt.Id ";

    private static Person AttachType(Person person, PersonType personType)
    {
        if (personType.Id != 0)
            person.PersonType = personType;
        return person;
    }

    public Task<IEnumerable<Person>> GetAllAsync()
        => _db.QueryAsync<Person, PersonType, Person>(
            $"SELECT {PersonColumns}, {PersonTypeColumns}{JoinSql}ORDER BY p.Id;",
            AttachType,
            splitOn: "Id");

    public Task<Person?> GetByIdAsync(int id)
        => _db.QueryFirstOrDefaultAsync<Person, PersonType, Person>(
            $"SELECT {PersonColumns}, {PersonTypeColumns}{JoinSql}WHERE p.Id = @Id;",
            AttachType,
            splitOn: "Id",
            param: new { Id = id });

    public async Task<int> AddAsync(Person person)
    {
        // SQLite: last_insert_rowid() is connection-scoped.
        const string sql = @"INSERT INTO Person
(PersonTypeId, Name, LastName, Phone, Email, Address, RegisterDate, Active)
VALUES
(@PersonTypeId, @Name, @LastName, @Phone, @Email, @Address, @RegisterDate, @Active);
SELECT last_insert_rowid();";

        var newId = await _db.ExecuteScalarAsync<long>(sql, person);
        return (int)newId;
    }

    public Task<int> UpdateAsync(Person person)
        => _db.ExecuteAsync(
            "UPDATE Person SET PersonTypeId=@PersonTypeId, Name=@Name, LastName=@LastName, Phone=@Phone, Email=@Email, Address=@Address, RegisterDate=@RegisterDate, Active=@Active WHERE Id=@Id;",
            person);

    public Task<int> DeleteAsync(int id)
        => _db.ExecuteAsync("DELETE FROM Person WHERE Id=@Id;", new { Id = id });
}
