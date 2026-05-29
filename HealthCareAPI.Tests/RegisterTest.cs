using Microsoft.EntityFrameworkCore;
using HealthCare;
using HealthcareAPI.DTOs;
using HealthcareAPI.Services;

namespace HealthcareAPI.Tests;

public class RegisterTests
{
    [Fact]
    public async Task RegisterAsync_NewUser_SuccessfullyCreatesUser()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
        
        var request = new RegisterRequest
        {
            Name = "My Test User",
            Email = "MyTest@test.com",
            Password = "SecretCode123!!"
        };
        
        await authService.RegisterAsync(request);
        
        var savedUser = await context.Patients
            .FirstOrDefaultAsync(x => x.Email == "MyTest@test.com");
        
        Assert.NotNull(savedUser);
        Assert.Equal("My Test User", savedUser!.Name);
        Assert.NotEqual("SecretCode123!!", savedUser.PasswordHash);
    }
    
    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsException()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
        
        var request = new RegisterRequest
        {
            Name = "Duplicate User",
            Email = "existing@test.com",  // ← ÄNDRAD: använd seedad email
            Password = "Password123!"
        };
        
        var exception = await Assert.ThrowsAsync<Exception>(
            () => authService.RegisterAsync(request)
        );
        
        Assert.Equal("Email already exists", exception.Message);  // ← ÄNDRAD: rätt meddelande
    }
    
    [Fact]
    public async Task RegisterAsync_EmptyName_ShouldStillCreateUser()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var config = TestConfig.GetConfiguration();
        var authService = new AuthService(context, config);
        
        var request = new RegisterRequest
        {
            Name = "",
            Email = "noname@test.com",
            Password = "Password123!"
        };
        
        await authService.RegisterAsync(request);
        
        var savedUser = await context.Patients
            .FirstOrDefaultAsync(x => x.Email == "noname@test.com");
        
        Assert.NotNull(savedUser);
        Assert.Equal("", savedUser!.Name);
    }
}