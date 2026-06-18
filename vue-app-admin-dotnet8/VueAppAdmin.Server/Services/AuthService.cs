namespace VueAppAdmin.Server.Services;

public class AuthService
{
    public bool ValidateCredentials(string username, string password)
        => username == "admin" && password == "password";

    public string GetUserDisplayName(string username)
        => username == "admin" ? "Administrator" : username;
}
