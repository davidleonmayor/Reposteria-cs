namespace Api.Core.Shared.Settings;

public sealed record JwtSettings
{
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = "Api";
    public string Audience { get; init; } = "ApiUsers";
    public int ExpirationHours { get; init; } = 8;
}
