using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VueAppAdmin.Server.Features.Auth.Requests;
using VueAppAdmin.Server.Features.Auth.Responses;
using VueAppAdmin.Server.Shared;
using VueAppAdmin.Server.Shared.Jwt;

namespace VueAppAdmin.Server.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IJwtService jwtService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!authService.ValidateCredentials(request.Username, request.Password))
            return Unauthorized(ApiResponse<object>.Fail("帳號或密碼錯誤"));

        var displayName = authService.GetUserDisplayName(request.Username);
        var token = jwtService.GenerateToken(request.Username, displayName);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse { Token = token }, "登入成功"));
    }

    [HttpGet("Me")]
    public IActionResult Me()
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var displayName = User.FindFirstValue("displayName") ?? string.Empty;

        return Ok(ApiResponse<MeResponse>.Ok(new MeResponse
        {
            Username = username,
            DisplayName = displayName
        }));
    }
}
