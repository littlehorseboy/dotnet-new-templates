using VueAppAdmin.Server.Shared.Logging;

namespace VueAppAdmin.Server.Features.Auth;

public class AuthService : IAuthService
{
    private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<AuthService>();

    public bool ValidateCredentials(string username, string password)
    {
        _logger.Information("Login attempt for {Username}", username);

        return username == "admin" && password == "password";
    }

    public string GetUserDisplayName(string username)
        => username == "admin" ? "Administrator" : username;
}
