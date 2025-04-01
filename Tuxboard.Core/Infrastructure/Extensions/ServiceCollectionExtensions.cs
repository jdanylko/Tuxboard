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
    public static IServiceCollection AddTuxboardDashboard<T>(this IServiceCollection services, IConfiguration configuration) where T: struct
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(nameof(TuxboardConfig));
        TuxboardConfig appConfig = new();
        section.Bind(appConfig);
        services.AddSingleton<ITuxboardConfig>(appConfig);

        var assemblyName = Assembly.GetCallingAssembly()!.GetName().Name;

        // Tuxboard DbContext
        services.AddDbContext<TuxDbContext<T>>(options =>
        {
            options.UseSqlServer(appConfig.ConnectionString, x => x.MigrationsAssembly(assemblyName));
        });

        // For Dependency Injection
        services.AddTransient<IDashboardService<T>, DashboardService<T>>();
        services.AddTransient<ITuxDbContext<T>, TuxDbContext<T>>();

        return services;
    }

    /// <summary>
    /// Adds Tuxboard Dashboard services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="setupConfig">An <see cref="Action{TuxboardConfig}"/> to configure the TuxboardConfig.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="setupConfig"/> is null.</exception>
    public static IServiceCollection AddTuxboardDashboard<T>(this IServiceCollection services, Action<TuxboardConfig> setupConfig) where T: struct
    {
        ArgumentNullException.ThrowIfNull(setupConfig);

        TuxboardConfig appConfig = new();
        setupConfig(appConfig);
        services.AddSingleton<ITuxboardConfig>(appConfig);

        var assemblyName = Assembly.GetCallingAssembly()!.GetName().Name;

        services.AddDbContext<TuxDbContext<T>>(options =>
        {
            options.UseSqlServer(appConfig.ConnectionString, x => x.MigrationsAssembly(assemblyName));
        });

        services.AddTransient<IDashboardService<T>, DashboardService<T>>();
        services.AddTransient<ITuxDbContext<T>, TuxDbContext<T>>();

        return services;
    }
}