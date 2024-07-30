using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;

public partial class TuxDbContext : DbContext, ITuxDbContext
{
    private TuxboardConfig _tuxboardConfig;

    public TuxDbContext(DbContextOptions<TuxDbContext> options, IOptions<TuxboardConfig> config)
        : base(options)
    {
        _tuxboardConfig = config.Value;
    }

    public virtual DbSet<Dashboard> Dashboards { get; set; }
    public virtual DbSet<DashboardDefault> DashboardDefaults { get; set; }
    public virtual DbSet<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }
    public virtual DbSet<DashboardTab> DashboardTabs { get; set; }
    public virtual DbSet<Layout> Layouts { get; set; }
    public virtual DbSet<LayoutRow> LayoutRows { get; set; }
    public virtual DbSet<LayoutType> LayoutTypes { get; set; }
    public virtual DbSet<Plan> Plans { get; set; }
    public virtual DbSet<Widget> Widgets { get; set; }
    public virtual DbSet<WidgetDefault> WidgetDefaults { get; set; }
    public virtual DbSet<WidgetDefaultOption> WidgetDefaultOptions { get; set; }
    public virtual DbSet<WidgetPlacement> WidgetPlacements { get; set; }
    public virtual DbSet<WidgetSetting> WidgetSettings { get; set; }

    
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