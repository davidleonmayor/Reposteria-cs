using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class CreateSaleUseCase
{
    private readonly ISaleRepository _repository;
    public CreateSaleUseCase(ISaleRepository repository) => _repository = repository;

    public async Task<SaleResponse> ExecuteAsync(SaleCreateRequest request, CancellationToken ct = default)
    {
        // Validate persons exist
        var personIds = request.Participants.Select(p => p.PersonId).Distinct().ToList();
        var existingPersonIds = await _repository.GetExistingPersonIdsAsync(personIds, ct);
        var missingPersons = personIds.Except(existingPersonIds).ToList();
        if (missingPersons.Count > 0)
            throw new ArgumentException($"Persons not found: {string.Join(", ", missingPersons)}");

        // Validate products exist
        var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
        var products = await _repository.GetProductInfoAsync(productIds, ct);
        var missingProducts = productIds.Where(id => !products.ContainsKey(id)).ToList();
        if (missingProducts.Count > 0)
            throw new ArgumentException($"Products not found: {string.Join(", ", missingProducts)}");

        // Validate stock and build details
        var stockDeductions = new Dictionary<int, int>();
        var details = new List<global::SaleDetail>();
        foreach (var d in request.Details)
        {
            var info = products[d.ProductId];
            var unitPrice = d.UnitPrice ?? info.Price;
            var totalRequested = stockDeductions.GetValueOrDefault(d.ProductId) + d.Quantity;
            if (info.Stock < totalRequested)
                throw new ArgumentException(
                    $"Insufficient stock for product {d.ProductId}. Available: {info.Stock}, Requested: {totalRequested}");
            stockDeductions[d.ProductId] = totalRequested;
            details.Add(new global::SaleDetail
            {
                ProductId = d.ProductId,
                Quantity  = d.Quantity,
                UnitPrice = unitPrice
            });
        }

        var subtotal = details.Sum(x => x.UnitPrice * x.Quantity);

        var sale = new global::Sale
        {
            SaleDate     = request.SaleDate ?? DateTime.UtcNow,
            State        = StringNormalization.Clean(request.State),
            Observations = request.Observations is null ? null : StringNormalization.Clean(request.Observations),
            Subtotal     = subtotal,
            Total        = subtotal,
            Participants = request.Participants
                .Select(p => new global::SaleParticipant
                {
                    PersonId = p.PersonId,
                    Role     = StringNormalization.Clean(p.Role)
                })
                .ToList(),
            Details = details
        };

        return await _repository.CreateAsync(sale, stockDeductions, ct);
    }
}
