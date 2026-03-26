using Api.Core.Modules.Auth.Application.DTOs;
using Api.Core.Modules.Auth.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RegisterUseCase _registerUseCase;

    public AuthController(LoginUseCase loginUseCase, RegisterUseCase registerUseCase)
    {
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _loginUseCase.ExecuteAsync(request, cancellationToken);
        if (response is null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _registerUseCase.ExecuteAsync(request, cancellationToken);
        if (response is null)
            return Conflict(new { message = "Email already in use" });

        return CreatedAtAction(nameof(Login), new { request.Email }, response);
    }
}
