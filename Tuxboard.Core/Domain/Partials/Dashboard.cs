﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

public partial class Dashboard
{
    [NotMapped]
    public ITuxboardConfig Settings { get; set; }

    public static Dashboard Create(Guid? userId)
    {
        return new()
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
    }

    public List<Layout> GetLayouts(int tabIndex)
    {
        var tab = GetTab(tabIndex);
        return tab?.GetLayouts();
    }
        
    public LayoutRow GetLayoutRow(LayoutRow row)
    {
        return GetLayoutRow(row.LayoutRowId);
    }

    public LayoutRow GetLayoutRow(Guid layoutRowId)
    {
        var layouts = GetLayouts(GetCurrentTab().TabIndex);
        var layout = layouts.FirstOrDefault(e => e.LayoutRows.Any(t => t.LayoutRowId == layoutRowId));
        return layout.LayoutRows.FirstOrDefault(y => y.LayoutRowId == layoutRowId);
    }

    public Layout GetLayoutByLayoutRow(Guid layoutRowId)
    {
        var layouts = GetLayouts(GetCurrentTab().TabIndex);
        return layouts.FirstOrDefault(e => e.LayoutRows.Any(t => t.LayoutRowId == layoutRowId));
    }

    public bool RowContainsWidgets(LayoutRow row)
    {
        var tab = GetCurrentTab();
        return tab == null && tab.RowContainsWidgets(row);
    }

    public bool RowContainsWidgets(Guid rowId)
    {
        var tab = GetCurrentTab();
        return tab == null && tab.RowContainsWidgets(rowId);
    }

    public bool ContainsOneRow()
    {
        var tab = GetCurrentTab();
        // Should ALWAYS be one layout...for now.
        var layout = tab.Layouts.FirstOrDefault();
        return layout.ContainsOneRow();
    }

    public DashboardTab GetCurrentTab()
    {
        return GetTab(SelectedTab);
    }

    public DashboardTab GetTab(int tabIndex)
    {
        // Zero-Based!
        return Tabs.ElementAtOrDefault(tabIndex-1);
    }

    public DashboardDto ToDto()
    {
        return new DashboardDto
        {
            SelectedTab = this.SelectedTab,
            Tabs = Tabs.Select(tab => tab.ToDto())
                .ToList() // Tabs
        };
    }

    public LayoutRow GetFirstLayoutRow() => 
        GetCurrentTab()?.GetLayouts()?.FirstOrDefault()?.LayoutRows?.FirstOrDefault();

    public LayoutRow GetLayoutRowByIndex(int index) => 
        GetCurrentTab()?.GetLayouts()?.FirstOrDefault()?.LayoutRows.ElementAt(index);
}