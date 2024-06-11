using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Data.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;

public class TuxDbContext : DbContext, ITuxDbContext
{
    private IConfiguration _config;

    public TuxDbContext(DbContextOptions<TuxDbContext> options, IConfiguration config)
        : base(options)
    {
            _config = config;
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

            var tuxboardConfig = _config.GetSection(nameof(TuxboardConfig));

            if (tuxboardConfig.Exists())
            {
                var schema = string.IsNullOrEmpty(tuxboardConfig[nameof(TuxboardConfig.Schema)]) 
                    ? "dbo" 
                    : tuxboardConfig[nameof(TuxboardConfig.Schema)];
                modelBuilder.HasDefaultSchema(schema);
            }

            modelBuilder.ApplyConfiguration(new DashboardConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardDefaultConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardDefaultWidgetConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardTabConfiguration());
            modelBuilder.ApplyConfiguration(new LayoutConfiguration());
            modelBuilder.ApplyConfiguration(new LayoutRowConfiguration());
            modelBuilder.ApplyConfiguration(new LayoutTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PlanConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetDefaultConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetDefaultOptionConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetPlacementConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetSettingConfiguration());
        }
}