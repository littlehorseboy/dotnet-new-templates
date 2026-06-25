using Microsoft.Extensions.Logging;
using NSubstitute;
using VueAppAdmin.Server.Features.Auth;

namespace VueAppAdmin.Server.Tests.Features.Auth;

public class AuthServiceTests
{
    private readonly ILogger<AuthService> _logger = Substitute.For<ILogger<AuthService>>();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_logger);
    }

    [Fact]
    public void ValidateCredentials_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = _sut.ValidateCredentials("admin", "password");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateCredentials_WrongPassword_ReturnsFalse()
    {
        // Arrange
        // Act
        var result = _sut.ValidateCredentials("admin", "wrong");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateCredentials_UnknownUser_ReturnsFalse()
    {
        // Arrange
        // Act
        var result = _sut.ValidateCredentials("unknown", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetUserDisplayName_AdminUser_ReturnsAdministrator()
    {
        // Arrange
        // Act
        var result = _sut.GetUserDisplayName("admin");

        // Assert
        Assert.Equal("Administrator", result);
    }
}
