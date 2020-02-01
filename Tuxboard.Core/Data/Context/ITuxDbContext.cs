using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context
{
    public interface ITuxDbContext
    {
        DbSet<Dashboard> Dashboard { get; set; }
        DbSet<DashboardDefault> DashboardDefault { get; set; }
        DbSet<DashboardDefaultWidget> DashboardDefaultWidget { get; set; }
        DbSet<DashboardTab> DashboardTab { get; set; }
        DbSet<Layout> Layout { get; set; }
        DbSet<LayoutRow> LayoutRow { get; set; }
        DbSet<LayoutType> LayoutType { get; set; }
        DbSet<Widget> Widget { get; set; }
        DbSet<WidgetDefault> WidgetDefault { get; set; }
        DbSet<WidgetDefaultOption> WidgetDefaultOption { get; set; }
        DbSet<WidgetPlacement> WidgetPlacement { get; set; }
        DbSet<WidgetSetting> WidgetSetting { get; set; }
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
    }
}