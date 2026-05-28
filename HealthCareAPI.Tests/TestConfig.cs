using Microsoft.Extensions.Configuration;

namespace HealthcareAPI.Tests;

public static class TestConfig
{
    public static IConfiguration GetConfiguration()
    {
        var config = new Dictionary<string, string>
        {
            {"Jwt:Key", "this-is-a-test-key-with-32-characters!!!"},
            {"Jwt:Issuer", "test-issuer"},
            {"Jwt:Audience", "test-audience"}
        };
        
        return new ConfigurationBuilder()
            .AddInMemoryCollection(config!)
            .Build();
    }
}
