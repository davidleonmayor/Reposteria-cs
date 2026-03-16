public sealed class SaleParticipantRepository : ISaleParticipantRepository
{
    private readonly IDapperHelper _db;

    public SaleParticipantRepository(IDapperHelper db) => _db = db;

    // First "Id" -> SaleParticipant.Id; second "Id" -> Person.Id (splitOn).
    private const string ParticipantColumns =
        "sp.Id, sp.SaleId, sp.PersonId, sp.Role";

    private const string PersonColumns =
        "p.Id, p.PersonTypeId, p.Name, p.LastName, p.Phone, p.Email, p.Address, p.RegisterDate, p.Active";

    private const string JoinSql =
        " FROM SaleParticipant sp LEFT JOIN Person p ON sp.PersonId = p.Id ";

    private static SaleParticipant AttachPerson(SaleParticipant participant, Person person)
    {
        if (person.Id != 0)
            participant.Person = person;
        return participant;
    }

    public Task<IEnumerable<SaleParticipant>> GetAllAsync()
        => _db.QueryAsync<SaleParticipant, Person, SaleParticipant>(
            $"SELECT {ParticipantColumns}, {PersonColumns}{JoinSql}ORDER BY sp.Id;",
            AttachPerson,
            splitOn: "Id");

    public Task<IEnumerable<SaleParticipant>> GetBySaleIdAsync(int saleId)
        => _db.QueryAsync<SaleParticipant, Person, SaleParticipant>(
            $"SELECT {ParticipantColumns}, {PersonColumns}{JoinSql}WHERE sp.SaleId = @SaleId ORDER BY sp.Id;",
            AttachPerson,
            splitOn: "Id",
            param: new { SaleId = saleId });

    public Task<SaleParticipant?> GetByIdAsync(int id)
        => _db.QueryFirstOrDefaultAsync<SaleParticipant, Person, SaleParticipant>(
            $"SELECT {ParticipantColumns}, {PersonColumns}{JoinSql}WHERE sp.Id = @Id;",
            AttachPerson,
            splitOn: "Id",
            param: new { Id = id });

    public async Task<int> AddAsync(SaleParticipant participant)
    {
        const string sql = @"INSERT INTO SaleParticipant (SaleId, PersonId, Role)
VALUES (@SaleId, @PersonId, @Role);
SELECT last_insert_rowid();";

        var newId = await _db.ExecuteScalarAsync<long>(sql, new
        {
            participant.SaleId,
            participant.PersonId,
            participant.Role
        });
        return (int)newId;
    }

    public Task<int> UpdateAsync(SaleParticipant participant)
        => _db.ExecuteAsync(
            "UPDATE SaleParticipant SET SaleId=@SaleId, PersonId=@PersonId, Role=@Role WHERE Id=@Id;",
            new
            {
                participant.Id,
                participant.SaleId,
                participant.PersonId,
                participant.Role
            });

    public Task<int> DeleteAsync(int id)
        => _db.ExecuteAsync("DELETE FROM SaleParticipant WHERE Id=@Id;", new { Id = id });
}
