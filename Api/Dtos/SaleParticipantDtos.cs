using System.ComponentModel.DataAnnotations;

/// <summary>DTO para crear un participante de venta independiente.</summary>
public sealed class SaleParticipantCreateDto
{
    /// <summary>ID de la venta a la que pertenece el participante.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "SaleId must be a positive integer.")]
    public int SaleId { get; set; }

    /// <summary>ID de la persona participante.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "PersonId must be a positive integer.")]
    public int PersonId { get; set; }

    /// <summary>Rol del participante en la venta (ej. Cliente, Vendedor).</summary>
    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}

/// <summary>DTO para actualizar un participante de venta existente.</summary>
public sealed class SaleParticipantUpdateDto
{
    /// <summary>ID de la venta a la que pertenece el participante.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "SaleId must be a positive integer.")]
    public int SaleId { get; set; }

    /// <summary>ID de la persona participante.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "PersonId must be a positive integer.")]
    public int PersonId { get; set; }

    /// <summary>Rol del participante en la venta (ej. Cliente, Vendedor).</summary>
    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}
