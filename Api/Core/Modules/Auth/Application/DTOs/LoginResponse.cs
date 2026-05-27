using System;

namespace Api.Core.Modules.Auth.Application.DTOs;

public sealed record LoginResponse(string Token, DateTime ExpiresAt, int PersonId, string Role);
