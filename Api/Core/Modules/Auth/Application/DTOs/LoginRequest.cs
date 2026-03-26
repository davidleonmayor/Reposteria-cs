namespace Api.Core.Modules.Auth.Application.DTOs;

public sealed record LoginRequest(string Email, string Password);
