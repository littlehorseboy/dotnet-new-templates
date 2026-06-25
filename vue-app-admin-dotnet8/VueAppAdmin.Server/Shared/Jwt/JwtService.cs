using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VueAppAdmin.Server.Shared.Jwt;

public class JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger) : IJwtService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(string username, string displayName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("displayName", displayName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SignKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogInformation("Token generated for {Username}", username);

        return tokenString;
    }
}
