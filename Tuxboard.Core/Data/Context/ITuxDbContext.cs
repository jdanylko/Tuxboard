using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;

public interface ITuxDbContext
{
    DbSet<Dashboard> Dashboards { get; set; }
    DbSet<DashboardDefault> DashboardDefaults { get; set; }
    DbSet<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }
    DbSet<DashboardTab> DashboardTabs { get; set; }
    DbSet<Layout> Layouts { get; set; }
    DbSet<LayoutRow> LayoutRows { get; set; }
    DbSet<LayoutType> LayoutTypes { get; set; }
    DbSet<Widget> Widgets { get; set; }
    DbSet<WidgetDefault> WidgetDefaults { get; set; }
    DbSet<WidgetDefaultOption> WidgetDefaultOptions { get; set; }
    DbSet<WidgetPlacement> WidgetPlacements { get; set; }
    DbSet<WidgetSetting> WidgetSettings { get; set; }
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    int SaveChanges();
    int SaveChanges(bool acceptAllChangesOnSuccess);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);

}