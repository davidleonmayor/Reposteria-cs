using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.Interfaces;

namespace Api.Core.Modules.SaleDetails.Application.UseCases;

public sealed class CreateSaleDetailUseCase
{
    private readonly ISaleDetailRepository _repository;
    public CreateSaleDetailUseCase(ISaleDetailRepository repository) => _repository = repository;

    public async Task<SaleDetailResponse> ExecuteAsync(SaleDetailCreateRequest request, CancellationToken ct = default)
    {
        // Validate sale exists
        if (!await _repository.SaleExistsAsync(request.SaleId, ct))
            throw new ArgumentException($"Sale {request.SaleId} not found.");

        // Validate product exists and has sufficient stock
        var info = await _repository.GetProductInfoAsync(request.ProductId, ct)
            ?? throw new ArgumentException($"Product {request.ProductId} not found.");

        if (info.Stock < request.Quantity)
            throw new ArgumentException(
                $"Insufficient stock for product {request.ProductId}. Available: {info.Stock}, Requested: {request.Quantity}");

        var detail = new global::SaleDetail
        {
            SaleId    = request.SaleId,
            ProductId = request.ProductId,
            Quantity  = request.Quantity,
            UnitPrice = request.UnitPrice ?? info.Price
        };

        return await _repository.CreateAsync(detail, request.Quantity, ct);
    }
}
