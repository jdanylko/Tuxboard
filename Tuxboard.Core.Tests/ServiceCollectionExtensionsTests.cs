using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Infrastructure;
using Tuxboard.Core.Infrastructure.Interfaces;

namespace Tuxboard.Core.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTuxboardDashboard_WithValidConfiguration_AddsServices()
    {
        // Arrange
        const string jsonConfig = """
        {
            "TuxboardConfig": {
                "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=Tuxboard;Trusted_Connection=True;MultipleActiveResultSets=true",
                "MigrationAssembly": "Tuxboard.Core",
                "Schema": "dbo"
            }
        }
        """;

        // Load the JSON string into a stream
        using MemoryStream stream = new();
        using StreamWriter writer = new(stream);
        writer.Write(jsonConfig);
        writer.Flush();
        stream.Position = 0;

        // Add the JSON stream to the configuration builder
        IConfiguration configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
        ServiceCollection services = new();

        // Act
        services.AddTuxboardDashboard(configuration);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IDashboardService));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxDbContext));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxboardConfig));
    }

    [Fact]
    public void AddTuxboardDashboard_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddTuxboardDashboard(configuration));
    }

    [Fact]
    public void AddTuxboardDashboard_WithSetupConfig_AddsServices()
    {
        // Arrange

        Action<TuxboardConfig> setupConfig = config =>
        {
            config.ConnectionString = "SomeConnection";
            config.MigrationAssembly = "SomeAssembly";
        };
        ServiceCollection services = new();
        // Act
        services.AddTuxboardDashboard(setupConfig);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IDashboardService));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxDbContext));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxboardConfig));
    }

    [Fact]
    public void AddTuxboardDashboard_WithNullSetupConfig_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<TuxboardConfig>? setupConfig = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddTuxboardDashboard(setupConfig));
    }
}