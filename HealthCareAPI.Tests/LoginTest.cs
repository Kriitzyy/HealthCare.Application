using HealthCare;
using HealthcareAPI.DTOs;
using HealthcareAPI.Services;

namespace HealthcareAPI.Tests;

public class LoginTests
{
    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsToken()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
       
        var request = new LoginRequest
        {
            Email = "existing@test.com",  // ← ÄNDRAD: använd seedad email
            Password = "CorrectPassword123!"  // ← ÄNDRAD: använd rätt lösenord
        };
       
        var response = await authService.LoginAsync(request);
       
        Assert.NotNull(response);
        Assert.NotEmpty(response.Token);
        Assert.Equal("existing@test.com", response.Email);
        Assert.Equal("Existing User", response.Name);
    }
   
    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorized()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
       
        var request = new LoginRequest
        {
            Email = "existing@test.com",  // ← ÄNDRAD: använd seedad email
            Password = "WrongPassword!"
        };
       
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => authService.LoginAsync(request)
        );
    }
   
    [Fact]
    public async Task LoginAsync_NonExistentEmail_ThrowsUnauthorized()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
       
        var request = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "AnyPassword123!"
        };
       
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => authService.LoginAsync(request)
        );
    }
   
    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsValidJwtToken()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
       
        var request = new LoginRequest
        {
            Email = "existing@test.com",  // ← ÄNDRAD: använd seedad email
            Password = "CorrectPassword123!"  // ← ÄNDRAD: använd rätt lösenord
        };
       
        var response = await authService.LoginAsync(request);
       
        var tokenParts = response.Token.Split('.');
        Assert.Equal(3, tokenParts.Length);
    }
}