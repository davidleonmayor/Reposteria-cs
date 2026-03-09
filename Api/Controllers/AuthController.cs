using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config) => _config = config;

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var users = _config.GetSection("Auth:Users").Get<List<AuthUser>>() ?? new();
        var user = users.FirstOrDefault(u =>
            string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == request.Password);

        if (user is null)
            return Unauthorized(new { message = "Invalid credentials" });

        var issuer = _config["Jwt:Issuer"] ?? "Api";
        var audience = _config["Jwt:Audience"] ?? "ApiUsers";
        var keyStr = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyStr) || keyStr.Length < 32)
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "JWT key is not configured (min 32 chars)." });

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddHours(8);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in user.Roles ?? Array.Empty<string>())
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt = expires
        });
    }
}

public record LoginRequest(string Username, string Password);

public sealed class AuthUser
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string[]? Roles { get; init; }
}
