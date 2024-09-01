using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Context;
using Tuxboard.Core.Infrastructure.Services;

namespace Tuxboard.Core.Infrastructure;

/// <summary>
/// Extension methods for configuring Tuxboard Dashboard services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Tuxboard Dashboard services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public static IServiceCollection AddTuxboardDashboard(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        IConfigurationSection section = configuration.GetSection(nameof(TuxboardConfig));
        TuxboardConfig appConfig = new();
        section.Bind(appConfig);
        services.AddSingleton<ITuxboardConfig>(appConfig);

        string assemblyName = Assembly.GetCallingAssembly()!.GetName().Name;

        // Tuxboard DbContext
        services.AddDbContext<TuxDbContext>(options =>
        {
            options.UseSqlServer(appConfig.ConnectionString, x => x.MigrationsAssembly(assemblyName));
        });

        // For Dependency Injection
        services.AddTransient<IDashboardService, DashboardService>();
        services.AddTransient<ITuxDbContext, TuxDbContext>();

        return services;
    }

    /// <summary>
    /// Adds Tuxboard Dashboard services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="setupConfig">An <see cref="Action{TuxboardConfig}"/> to configure the TuxboardConfig.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="setupConfig"/> is null.</exception>
    public static IServiceCollection AddTuxboardDashboard(this IServiceCollection services, Action<TuxboardConfig> setupConfig)
    {
        ArgumentNullException.ThrowIfNull(setupConfig);

        TuxboardConfig appConfig = new();
        setupConfig(appConfig);
        services.AddSingleton<ITuxboardConfig>(appConfig);

        string assemblyName = Assembly.GetCallingAssembly()!.GetName().Name;

        services.AddDbContext<TuxDbContext>(options =>
        {
            options.UseSqlServer(appConfig.ConnectionString, x => x.MigrationsAssembly(assemblyName));
        });

        services.AddTransient<IDashboardService, DashboardService>();
        services.AddTransient<ITuxDbContext, TuxDbContext>();

        return services;
    }
}