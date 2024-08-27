using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class Dashboard
{
    /// <summary>
    /// Configuration for a dashboard
    /// </summary>
    [NotMapped]
    public ITuxboardConfig Settings { get; set; }

    /// <summary>
    /// Create a new dashboard for a new user
    /// </summary>
    /// <param name="userId">UserID</param>
    /// <returns>Dashboard shell</returns>
    public static Dashboard Create(Guid? userId) =>
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
    /// <returns>List of Layouts</returns>
    public List<Layout> GetLayouts(int tabIndex)
    {
        var tab = GetTab(tabIndex);
        return tab?.GetLayouts();
    }
        
    /// <summary>
    /// Locate a LayoutRow based on a LayoutRowId
    /// </summary>
    /// <param name="layoutRowId">LayoutRowId</param>
    /// <returns>LayoutRow if found, null is not found</returns>
    public LayoutRow GetLayoutRow(Guid layoutRowId)
    {
        var layouts = GetLayouts(GetCurrentTab().TabIndex);
        var layout = layouts.FirstOrDefault(e => e.LayoutRows.Any(t => t.LayoutRowId == layoutRowId));
        return layout.LayoutRows.FirstOrDefault(y => y.LayoutRowId == layoutRowId);
    }

    /// <summary>
    /// Return a Layout by a LayoutRowId
    /// </summary>
    /// <param name="layoutRowId">LayoutRowId Guid</param>
    /// <returns>Instance of a Layout if found, null if not found.</returns>
    public Layout GetLayoutByLayoutRow(Guid layoutRowId)
    {
        var layouts = GetLayouts(GetCurrentTab().TabIndex);
        return layouts.FirstOrDefault(e => e.LayoutRows.Any(t => t.LayoutRowId == layoutRowId));
    }

    /// <summary>
    /// Identify whether a LayoutRow contains ANY widgets
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool RowContainsWidgets(LayoutRow row) => row.RowContainsWidgets();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowId"></param>
    /// <returns></returns>
    public bool RowContainsWidgets(Guid rowId)
    {
        var tab = GetCurrentTab();
        return tab == null && tab.RowContainsWidgets(rowId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ContainsOneRow()
    {
        var tab = GetCurrentTab();
        // Should ALWAYS be one layout...for now.
        var layout = tab.Layouts.FirstOrDefault();
        return layout.ContainsOneRow();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public DashboardTab GetCurrentTab()
    {
        return GetTab(SelectedTab);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tabIndex"></param>
    /// <returns></returns>
    public DashboardTab GetTab(int tabIndex)
    {
        // Zero-Based!
        return Tabs.ElementAtOrDefault(tabIndex-1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <returns></returns>
    public LayoutRow GetFirstLayoutRow() => 
        GetCurrentTab()?.GetLayouts()?.FirstOrDefault()?.LayoutRows?.FirstOrDefault();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public LayoutRow GetLayoutRowByIndex(int index) => 
        GetCurrentTab()?.GetLayouts()?.FirstOrDefault()?.LayoutRows.ElementAt(index);
}