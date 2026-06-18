using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VueAppAdmin.Server.DTO.Request.Auth;
using VueAppAdmin.Server.DTO.Response.Auth;
using VueAppAdmin.Server.Services;

namespace VueAppAdmin.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService, JwtService jwtService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!authService.ValidateCredentials(request.Username, request.Password))
            return Unauthorized();

        var displayName = authService.GetUserDisplayName(request.Username);
        var token = jwtService.GenerateToken(request.Username, displayName);

        return Ok(new LoginResponse { Token = token });
    }

    [HttpGet("Me")]
    public IActionResult Me()
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var displayName = User.FindFirstValue("displayName") ?? string.Empty;

        return Ok(new { username, displayName });
    }
}
