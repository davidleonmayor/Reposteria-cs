public sealed class SaleDetailRepository : ISaleDetailRepository
{
    private readonly IDapperHelper _db;

    public SaleDetailRepository(IDapperHelper db) => _db = db;

    // SaleDetail columns: aliases needed so Dapper splitOn="Id" works correctly.
    // First set of "Id" -> SaleDetail.Id; second "Id" -> Product.Id.
    private const string DetailColumns =
        "sd.Id, sd.SaleId, sd.ProductId, sd.Quantity, sd.UnitPrice";

    private const string ProductColumns =
        "p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.Active";

    private const string JoinSql =
        " FROM SaleDetail sd LEFT JOIN Product p ON sd.ProductId = p.Id ";

    private static SaleDetail AttachProduct(SaleDetail detail, Product product)
    {
        if (product.Id != 0)
            detail.Product = product;
        return detail;
    }

    public Task<IEnumerable<SaleDetail>> GetAllAsync()
        => _db.QueryAsync<SaleDetail, Product, SaleDetail>(
            $"SELECT {DetailColumns}, {ProductColumns}{JoinSql}ORDER BY sd.Id;",
            AttachProduct,
            splitOn: "Id");

    public Task<IEnumerable<SaleDetail>> GetBySaleIdAsync(int saleId)
        => _db.QueryAsync<SaleDetail, Product, SaleDetail>(
            $"SELECT {DetailColumns}, {ProductColumns}{JoinSql}WHERE sd.SaleId = @SaleId ORDER BY sd.Id;",
            AttachProduct,
            splitOn: "Id",
            param: new { SaleId = saleId });

    public Task<SaleDetail?> GetByIdAsync(int id)
        => _db.QueryFirstOrDefaultAsync<SaleDetail, Product, SaleDetail>(
            $"SELECT {DetailColumns}, {ProductColumns}{JoinSql}WHERE sd.Id = @Id;",
            AttachProduct,
            splitOn: "Id",
            param: new { Id = id });

    public async Task<int> AddAsync(SaleDetail detail)
    {
        // Validate FK: Sale and Product must exist before insert.
        const string sql = @"INSERT INTO SaleDetail (SaleId, ProductId, Quantity, UnitPrice)
VALUES (@SaleId, @ProductId, @Quantity, @UnitPrice);
SELECT last_insert_rowid();";

        var newId = await _db.ExecuteScalarAsync<long>(sql, new
        {
            detail.SaleId,
            detail.ProductId,
            detail.Quantity,
            detail.UnitPrice
        });
        return (int)newId;
    }

    public Task<int> UpdateAsync(SaleDetail detail)
        => _db.ExecuteAsync(
            "UPDATE SaleDetail SET SaleId=@SaleId, ProductId=@ProductId, Quantity=@Quantity, UnitPrice=@UnitPrice WHERE Id=@Id;",
            new
            {
                detail.Id,
                detail.SaleId,
                detail.ProductId,
                detail.Quantity,
                detail.UnitPrice
            });

    public Task<int> DeleteAsync(int id)
        => _db.ExecuteAsync("DELETE FROM SaleDetail WHERE Id=@Id;", new { Id = id });
}
