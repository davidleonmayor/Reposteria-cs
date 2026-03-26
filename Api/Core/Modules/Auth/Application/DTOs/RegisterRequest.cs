namespace Api.Core.Modules.Auth.Application.DTOs;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string Name,
    string LastName,
    int PersonTypeId,
    string? Phone = null,
    string? Address = null);
