using System.ComponentModel.DataAnnotations;

/// <summary>DTO para crear un detalle de venta independiente.</summary>
public sealed class SaleDetailCreateDto
{
    /// <summary>ID de la venta a la que pertenece el detalle.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "SaleId must be a positive integer.")]
    public int SaleId { get; set; }

    /// <summary>ID del producto incluido en el detalle.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive integer.")]
    public int ProductId { get; set; }

    /// <summary>Cantidad de unidades (mínimo 1).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    /// <summary>Precio unitario al momento de la venta (mínimo 0).</summary>
    [Range(0, 1_000_000, ErrorMessage = "UnitPrice must be between 0 and 1,000,000.")]
    public decimal UnitPrice { get; set; }
}

/// <summary>DTO para actualizar un detalle de venta existente.</summary>
public sealed class SaleDetailUpdateDto
{
    /// <summary>ID de la venta a la que pertenece el detalle.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "SaleId must be a positive integer.")]
    public int SaleId { get; set; }

    /// <summary>ID del producto incluido en el detalle.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive integer.")]
    public int ProductId { get; set; }

    /// <summary>Cantidad de unidades (mínimo 1).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    /// <summary>Precio unitario al momento de la venta (mínimo 0).</summary>
    [Range(0, 1_000_000, ErrorMessage = "UnitPrice must be between 0 and 1,000,000.")]
    public decimal UnitPrice { get; set; }
}
