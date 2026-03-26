using System;

namespace Api.Core.Modules.Auth.Application.Models;

public sealed record TokenResult(string Token, DateTime ExpiresAt);
