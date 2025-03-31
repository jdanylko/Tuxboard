using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Data.Context;


/// <summary>
/// Interface for <see cref="TuxDbContext{TUserId}"/>
/// </summary>
public interface ITuxDbContext<TUserId> : ITuxDbContext where TUserId : struct
{
    /// <summary>
    /// <see cref="Dashboards"/> is the table storing all dashboards associated with a user.
    /// </summary>
    DbSet<Dashboard<TUserId>> Dashboards { get; set; }
}

/// <summary>
/// 
/// </summary>
public interface ITuxDbContext
{
    /// <summary>
    /// <see cref="DashboardTabs"/> contains tabs for every <see cref="Dashboard{T}"/>. Currently, only one tab should exist per dashboard.
    /// </summary>
    DbSet<DashboardTab> DashboardTabs { get; set; }

    /// <summary>
    /// <see cref="DashboardDefaults"/> is where pre-made dashboards are created for when users log into a system;
    /// One of two tables storing default dashboard information.
    /// The second table is <see cref="DashboardDefaultWidgets"/>.
    /// </summary>
    DbSet<DashboardDefault> DashboardDefaults { get; set; }

    /// <summary>
    /// <see cref="DashboardDefaultWidgets"/> is where pre-made widgets are attached to a dashboard when users log into a system;
    /// Two of two tables storing default dashboard information.
    /// The first table uses <see cref="DashboardDefaults"/> as the header.
    /// </summary>
    DbSet<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }

    /// <summary>
    /// <see cref="Layouts"/> contains a single <see cref="Layout"/> for a single tab; Only one <see cref="Layout"/> should exist for one <see cref="DashboardTab"/>.
    /// </summary>
    DbSet<Layout> Layouts { get; set; }

    /// <summary>
    /// <see cref="LayoutRows"/> contains one or many <see cref="LayoutRow"/>s per <see cref="Layout"/>. A <see cref="Layout"/> can have unlimited number of <see cref="LayoutRow"/>s.
    /// </summary>
    DbSet<LayoutRow> LayoutRows { get; set; }


    /// <summary>
    /// Each <see cref="LayoutType"/> defines the columns of a <see cref="LayoutRow"/>.
    /// </summary>
    DbSet<LayoutType> LayoutTypes { get; set; }

    /// <summary>
    /// <see cref="Widget"/> contains every widget used throughout a Tuxboard dashboard; Used to create <see cref="WidgetPlacement"/> records.
    /// </summary>
    DbSet<Widget> Widgets { get; set; }

    /// <summary>
    /// <see cref="WidgetDefaults"/> contains the default values for widgets. If you want to attach a default setting to a widget,
    /// associate a new record to a widget using the WidgetId and name the setting with a default value.
    /// </summary>
    DbSet<WidgetDefault> WidgetDefaults { get; set; }

    /// <summary>
    /// <see cref="WidgetDefaultOptions"/> is a table providing multiple options for a widget setting if the setting is a dropdown input.
    /// </summary>
    DbSet<WidgetDefaultOption> WidgetDefaultOptions { get; set; }

    /// <summary>
    /// <see cref="WidgetPlacements"/> contains every widget used on a user's dashboard.
    /// <see cref="WidgetPlacement"/> contains a WidgetId to refer to the initial widget when created.
    /// In programming terms, a <see cref="Widget"/> is considered an abstract class and a <see cref="WidgetPlacement"/> is inherited from the widget.
    /// </summary>
    DbSet<WidgetPlacement> WidgetPlacements { get; set; }

    /// <summary>
    /// <see cref="WidgetSettings"/> is similar to a Widget&lt;-&gt;WidgetPlacement relationship, but is between a WidgetSetting&lt;-&gt;WidgetDefault.
    /// On initial creation of a <see cref="WidgetPlacement"/>, it uses a <see cref="Widget"/> and pulls every setting in
    /// each <see cref="WidgetDefault"/> to create the <see cref="WidgetPlacement"/> with it's appropriate settings.
    /// </summary>
    DbSet<WidgetSetting> WidgetSettings { get; set; }

    /// <summary>
    /// Used for direct database calls.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// bubble-up the ChangeTracker...just in case. ;-)
    /// </summary>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Exposing the SaveChanges for synchronous saves
    /// </summary>
    /// <returns>Returning total number of records affected</returns>
    int SaveChanges();

    /// <summary>
    /// Exposing the SaveChanges for synchronous saves
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <returns>Returning total number of records affected</returns>
    int SaveChanges(bool acceptAllChangesOnSuccess);

    /// <summary>
    /// Exposing the SaveChangesAsync for asynchronous saves
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Exposing the SaveChangesAsync for asynchronous saves
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
}
