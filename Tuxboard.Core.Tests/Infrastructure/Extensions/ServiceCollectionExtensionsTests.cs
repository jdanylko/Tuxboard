using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Infrastructure;
using Tuxboard.Core.Infrastructure.Services;
using System.Reflection;
using System.Linq;

namespace Tuxboard.Core.Tests.Infrastructure.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTuxboardDashboard_WithValidConfiguration_AddsServices()
    {
        // Arrange
        const string connectionString = "Server=MyServer;Database=Tuxboard;Trusted_Connection=True;MultipleActiveResultSets=true";
        const string schema = "MySchema";
        var jsonConfig = $$$"""
                            {
                                "TuxboardConfig": {
                                    "ConnectionString": "{{{connectionString}}}",
                                    "Schema": "{{{schema}}}"
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
        services.AddTuxboardDashboard<int>(configuration);
        var provider = services.BuildServiceProvider();
        var config = provider.GetService<ITuxboardConfig>();

        // Assert
        Assert.NotNull(config);
        Assert.Equal(connectionString, config.ConnectionString);
        Assert.Equal(schema, config.Schema);
        Assert.Contains(services, d => d.ServiceType == typeof(IDashboardService<int>));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxDbContext<int>));
    }

    [Fact]
    public void AddTuxboardDashboard_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var method = typeof(ServiceCollectionExtensions).GetMethods().First(m => m.Name == "AddTuxboardDashboard" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType == typeof(IConfiguration)).MakeGenericMethod(typeof(int));
        object? nullArg = null;
        object?[] args = [services, nullArg];

        // Act & Assert
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, args));
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }

    [Fact]
    public void AddTuxboardDashboard_WithSetupConfig_AddsServices()
    {
        // Arrange
        const string connectionString = "SomeConnection";
        const string schema = "some-schema";
        Action<TuxboardConfig> setupConfig = config =>
        {
            config.ConnectionString = connectionString;
            config.Schema = schema;
        };
        ServiceCollection services = new();
        // Act
        services.AddTuxboardDashboard<int>(setupConfig);
        var provider = services.BuildServiceProvider();
        var config = provider.GetService<ITuxboardConfig>();

        // Assert
        Assert.NotNull(config);
        Assert.Equal(connectionString, config.ConnectionString);
        Assert.Equal(schema, config.Schema);
        Assert.Contains(services, d => d.ServiceType == typeof(IDashboardService<int>));
        Assert.Contains(services, d => d.ServiceType == typeof(ITuxDbContext<int>));
    }

    [Fact]
    public void AddTuxboardDashboard_WithNullSetupConfig_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var method = typeof(ServiceCollectionExtensions).GetMethods().First(m => m.Name == "AddTuxboardDashboard" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType == typeof(Action<TuxboardConfig>)).MakeGenericMethod(typeof(int));
        object? nullArg = null;
        object?[] args = [services, nullArg];
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, args));
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }
}