using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// CRUD de detalles de venta (<see cref="SaleDetail"/>).
/// Los detalles también se crean automáticamente al crear una <see cref="Sale"/>
/// vía <c>POST /api/Sale</c>; este controller permite gestionarlos de forma independiente.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SaleDetailController : ControllerBase
{
    private readonly ISaleDetailRepository _repo;

    public SaleDetailController(ISaleDetailRepository repo) => _repo = repo;

    /// <summary>Obtiene todos los detalles de venta.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllAsync());

    /// <summary>Obtiene todos los detalles pertenecientes a una venta específica.</summary>
    /// <param name="saleId">ID de la venta.</param>
    [HttpGet("sale/{saleId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySaleId(int saleId)
        => Ok(await _repo.GetBySaleIdAsync(saleId));

    /// <summary>Obtiene un detalle de venta por su ID.</summary>
    /// <param name="id">ID del detalle.</param>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _repo.GetByIdAsync(id);
        return detail is null ? NotFound() : Ok(detail);
    }

    /// <summary>Crea un nuevo detalle de venta. Requiere rol Admin o Seller (política SalesWrite).</summary>
    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleDetailCreateDto dto)
    {
        var detail = new SaleDetail
        {
            SaleId    = dto.SaleId,
            ProductId = dto.ProductId,
            Quantity  = dto.Quantity,
            UnitPrice = dto.UnitPrice
        };

        detail.Id = await _repo.AddAsync(detail);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    /// <summary>Actualiza un detalle de venta existente. Requiere rol Admin o Seller (política SalesWrite).</summary>
    /// <param name="id">ID del detalle a actualizar.</param>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleDetailUpdateDto dto)
    {
        var detail = await _repo.GetByIdAsync(id);
        if (detail is null) return NotFound();

        detail.SaleId    = dto.SaleId;
        detail.ProductId = dto.ProductId;
        detail.Quantity  = dto.Quantity;
        detail.UnitPrice = dto.UnitPrice;

        await _repo.UpdateAsync(detail);
        return NoContent();
    }

    /// <summary>Elimina un detalle de venta. Requiere rol Admin o Seller (política SalesWrite).</summary>
    /// <param name="id">ID del detalle a eliminar.</param>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var affected = await _repo.DeleteAsync(id);
        return affected == 0 ? NotFound() : NoContent();
    }
}
