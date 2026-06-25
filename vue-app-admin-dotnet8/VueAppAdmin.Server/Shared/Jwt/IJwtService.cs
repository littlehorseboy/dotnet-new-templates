namespace VueAppAdmin.Server.Shared.Jwt;

public interface IJwtService
{
    string GenerateToken(string username, string displayName);
}
