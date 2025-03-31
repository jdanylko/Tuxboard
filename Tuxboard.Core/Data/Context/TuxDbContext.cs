using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;

/// <summary>
/// <see cref="TuxDbContext"/> uses Entity Framework for storing and managing Tuxboard dashboards.
/// </summary>
public partial class TuxDbContext : DbContext, ITuxDbContext<int>
{
    private TuxboardConfig _tuxboardConfig;

    /// <inheritdoc />
    public TuxDbContext(DbContextOptions<TuxDbContext> options, IOptions<TuxboardConfig> config)
        : base(options)
    {
        _tuxboardConfig = config.Value;
    }

    /// <inheritdoc />
    public virtual DbSet<Dashboard<int>> Dashboards { get; set; }

    /// <inheritdoc />
    public virtual DbSet<DashboardDefault> DashboardDefaults { get; set; }

    /// <inheritdoc />
    public virtual DbSet<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }

    /// <inheritdoc />
    public virtual DbSet<DashboardTab> DashboardTabs { get; set; }

    /// <inheritdoc />
    public virtual DbSet<Layout> Layouts { get; set; }

    /// <inheritdoc />
    public virtual DbSet<LayoutRow> LayoutRows { get; set; }

    /// <inheritdoc />
    public virtual DbSet<LayoutType> LayoutTypes { get; set; }

    /// <summary>
    /// <see cref="Plans"/> is an optional table containing an ID and Title. Refer to <see cref="Plan"/> for additional details.
    /// </summary>
    public virtual DbSet<Plan> Plans { get; set; }

    /// <inheritdoc />
    public virtual DbSet<Widget> Widgets { get; set; }

    /// <inheritdoc />
    public virtual DbSet<WidgetDefault> WidgetDefaults { get; set; }

    /// <inheritdoc />
    public virtual DbSet<WidgetDefaultOption> WidgetDefaultOptions { get; set; }

    /// <inheritdoc />
    public virtual DbSet<WidgetPlacement> WidgetPlacements { get; set; }

    /// <inheritdoc />
    public virtual DbSet<WidgetSetting> WidgetSettings { get; set; }


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _tuxboardConfig.Schema =
            string.IsNullOrEmpty(_tuxboardConfig.Schema)
                ? "dbo"
                : _tuxboardConfig.Schema;

        modelBuilder.HasDefaultSchema(_tuxboardConfig.Schema);

        modelBuilder.ApplyConfiguration(new DashboardConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new DashboardDefaultConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new DashboardDefaultWidgetConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new DashboardTabConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new LayoutConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new LayoutRowConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new LayoutTypeConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new PlanConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new WidgetConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new WidgetDefaultConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new WidgetDefaultOptionConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new WidgetPlacementConfiguration(_tuxboardConfig));
        modelBuilder.ApplyConfiguration(new WidgetSettingConfiguration(_tuxboardConfig));

    }
}