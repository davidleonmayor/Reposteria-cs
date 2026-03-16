using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// CRUD de participantes de venta (<see cref="SaleParticipant"/>).
/// Los participantes también se crean automáticamente al crear una <see cref="Sale"/>
/// vía <c>POST /api/Sale</c>; este controller permite gestionarlos de forma independiente.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SaleParticipantController : ControllerBase
{
    private readonly ISaleParticipantRepository _repo;

    public SaleParticipantController(ISaleParticipantRepository repo) => _repo = repo;

    /// <summary>Obtiene todos los participantes de venta.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllAsync());

    /// <summary>Obtiene todos los participantes pertenecientes a una venta específica.</summary>
    /// <param name="saleId">ID de la venta.</param>
    [HttpGet("sale/{saleId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySaleId(int saleId)
        => Ok(await _repo.GetBySaleIdAsync(saleId));

    /// <summary>Obtiene un participante de venta por su ID.</summary>
    /// <param name="id">ID del participante.</param>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var participant = await _repo.GetByIdAsync(id);
        return participant is null ? NotFound() : Ok(participant);
    }

    /// <summary>Crea un nuevo participante de venta. Requiere rol Admin o Seller (política SalesWrite).</summary>
    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleParticipantCreateDto dto)
    {
        var participant = new SaleParticipant
        {
            SaleId   = dto.SaleId,
            PersonId = dto.PersonId,
            Role     = StringNormalization.Clean(dto.Role)
        };

        participant.Id = await _repo.AddAsync(participant);
        return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
    }

    /// <summary>Actualiza un participante de venta existente. Requiere rol Admin o Seller (política SalesWrite).</summary>
    /// <param name="id">ID del participante a actualizar.</param>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleParticipantUpdateDto dto)
    {
        var participant = await _repo.GetByIdAsync(id);
        if (participant is null) return NotFound();

        participant.SaleId   = dto.SaleId;
        participant.PersonId = dto.PersonId;
        participant.Role     = StringNormalization.Clean(dto.Role);

        await _repo.UpdateAsync(participant);
        return NoContent();
    }

    /// <summary>Elimina un participante de venta. Requiere rol Admin o Seller (política SalesWrite).</summary>
    /// <param name="id">ID del participante a eliminar.</param>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var affected = await _repo.DeleteAsync(id);
        return affected == 0 ? NotFound() : NoContent();
    }
}
