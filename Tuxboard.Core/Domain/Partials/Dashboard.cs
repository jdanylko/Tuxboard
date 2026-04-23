using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Main instance of a Tuxboard dashboard
/// <code>
/// <para>Dashboard (<see cref="Dashboard{T}"/>)<br/>
/// +-- Dashboard Tab (<see cref="DashboardTab"/>)<br/>
/// |   +-- Layout (<see cref="Layout"/>)<br/>
/// |       +-- LayoutRow(s) (<see cref="LayoutRow"/>)<br/>
/// |           +-- LayoutType (<see cref="LayoutType"/>)<br/>
/// |           +-- WidgetPlacements (<see cref="WidgetPlacement"/>)<br/>
/// |               +-- WidgetSetting (<see cref="WidgetSetting"/>)<br/>
/// |               +-- Widget (<see cref="Widget"/>)<br/>
/// |                   +-- WidgetDefault (<see cref="WidgetDefault"/>)<br/>
/// +-- Dashboard Default (<see cref="DashboardDefault"/>)<br/>
///     +-- Dashboard Default Widgets (<see cref="DashboardDefaultWidget"/>)
/// </para>
/// </code>
/// </summary>
public partial class Dashboard<T>
{
    /// <summary>
    /// Configuration for a dashboard through a <see cref="ITuxboardConfig"/>
    /// </summary>
    [NotMapped]
    public ITuxboardConfig Settings { get; set; } = null!;

    /// <summary>
    /// Create a default dashboard for a new user
    /// </summary>
    /// <param name="userId"><see cref="Guid"/> - UserID</param>
    /// <returns><see cref="Dashboard{T}"/></returns>
    public static Dashboard<T> Create(T? userId) =>
        new()
        {
            SelectedTab = 1,
            Tabs = new List<DashboardTab>
            {
                new()
                {
                    TabIndex = 1,
                    TabTitle = "Default"
                }
            },
            UserId = userId
        };

    /// <summary>
    /// Return a list of layouts in a dashboard tab
    /// </summary>
    /// <param name="tabIndex">Index of the Dashboard Tab (should be 1)</param>
    /// <returns><see cref="List{Layout}"/></returns>
    public List<Layout>? GetLayouts(int tabIndex)
    {
        var tab = GetTab(tabIndex);
        return tab?.GetLayouts();
    }

    /// <summary>
    /// Locate a <see cref="LayoutRow"/> based on an id
    /// </summary>
    /// <param name="layoutRowId">LayoutRowId</param>
    /// <returns><see cref="LayoutRow"/> if found, null is not found</returns>
    public LayoutRow? GetLayoutRow(Guid layoutRowId)
    {
        var layout = GetCurrentTab().GetCurrentLayout();
        return layout.LayoutRows.FirstOrDefault(r => r.LayoutRowId == layoutRowId);
    }

    /// <summary>
    /// Return a <see cref="Layout"/> by an id
    /// </summary>
    /// <param name="layoutRowId"><see cref="Guid"/> - LayoutRowId</param>
    /// <returns><see cref="Layout"/> if found, null if not found.</returns>
    public Layout? GetLayoutByLayoutRow(Guid layoutRowId)
    {
        var layout = GetCurrentTab().GetCurrentLayout();
        return layout.LayoutRows.Any(r => r.LayoutRowId == layoutRowId) ? layout : null;
    }

    /// <summary>
    /// Identify whether a <see cref="LayoutRow"/> contains ANY widgets
    /// </summary>
    /// <param name="row"><see cref="LayoutRow"/></param>
    /// <returns>true if widgets exist in this row, false if not</returns>
    public bool RowContainsWidgets(LayoutRow row) => row.RowContainsWidgets();

    /// <summary>
    /// Returns whether a <see cref="LayoutRow"/> contains widgets
    /// </summary>
    /// <param name="rowId"><see cref="Guid"/> - LayoutRowId</param>
    /// <returns>true if widgets exist in this row, false if not</returns>
    public bool RowContainsWidgets(Guid rowId) => GetCurrentTab().RowContainsWidgets(rowId);

    /// <summary>
    /// Returns whether a <see cref="DashboardTab"/> contains one layout row
    /// </summary>
    /// <returns>true if a tab contains at least one row, false if not</returns>
    public bool ContainsOneRow()
    {
        // Should ALWAYS be one layout...for now.
        return GetCurrentTab().GetCurrentLayout().ContainsOneRow();
    }

    /// <summary>
    /// Returns the currently selected tab. Currently only one tab is supported per dashboard;
    /// multiple tabs are planned for a future release.
    /// </summary>
    /// <returns><see cref="DashboardTab"/></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the dashboard has no tabs, or <see cref="DashboardBase.SelectedTab"/> does not
    /// correspond to an existing tab.
    /// </exception>
    public DashboardTab GetCurrentTab()
    {
        return GetTab(SelectedTab)
            ?? throw new InvalidOperationException(
                Tabs.Count == 0
                    ? "Dashboard has no tabs."
                    : $"Dashboard has no tab at selected index {SelectedTab}.");
    }

    /// <summary>
    /// Return a <see cref="DashboardTab"/> from an indexed collection of Tabs
    /// </summary>
    /// <param name="tabIndex">1-based index of a dashboard tab</param>
    /// <returns><see cref="DashboardTab"/></returns>
    public DashboardTab? GetTab(int tabIndex)
    {
        // Convert 1-based tab index to 0-based collection index
        return Tabs.ElementAtOrDefault(tabIndex - 1);
    }

    /// <summary>
    /// Returns a <see cref="DashboardDto"/> (Data Transfer Object)
    /// </summary>
    /// <returns><see cref="DashboardDto"/></returns>
    public DashboardDto ToDto()
    {
        return new DashboardDto
        {
            SelectedTab = this.SelectedTab,
            Tabs = Tabs.Select(tab => tab.ToDto())
                .ToList() // Tabs
        };
    }

    /// <summary>
    /// Returns the first Layout row on a tab.
    /// </summary>
    /// <returns><see cref="LayoutRow"/></returns>
    public LayoutRow? GetFirstLayoutRow() => 
        GetCurrentTab().GetCurrentLayout().LayoutRows.FirstOrDefault();

    /// <summary>
    /// Returns a <see cref="LayoutRow"/> based on a zero-based indexer
    /// </summary>
    /// <param name="index">Integer</param>
    /// <returns><see cref="LayoutRow"/></returns>
    public LayoutRow? GetLayoutRowByIndex(int index) => 
        GetCurrentTab().GetCurrentLayout().LayoutRows.ElementAtOrDefault(index);
}