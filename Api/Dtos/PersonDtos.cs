using System.ComponentModel.DataAnnotations;

public sealed class PersonCreateDto
{
    [Range(1, int.MaxValue)]
    public int PersonTypeId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    public DateTime? RegisterDate { get; set; }

    public bool Active { get; set; } = true;
}

public sealed class PersonUpdateDto
{
    [Range(1, int.MaxValue)]
    public int PersonTypeId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    public DateTime RegisterDate { get; set; }

    public bool Active { get; set; }
}
