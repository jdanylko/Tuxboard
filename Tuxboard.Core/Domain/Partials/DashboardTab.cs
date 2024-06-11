using System;
using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

public partial class DashboardTab
{
    public DashboardTabDto ToDto()
    {
        return new DashboardTabDto
        {
            TabId = TabId,
            TabIndex = TabIndex,
            TabTitle = TabTitle,
            Layouts = Layouts.Select(layout => layout.ToDto())
                .OrderBy(t => t.LayoutIndex)
                .ToList(),
            WidgetPlacements = GetWidgetPlacements()
                .Select(wp => wp.ToDto())
                .ToList()
        };
    }

    public List<Layout> GetLayouts()
    {
        return Layouts.ToList();
    }

    public bool RowContainsWidgets(LayoutRow row)
    {
        return RowContainsWidgets(row.LayoutRowId);
    }

    public bool RowContainsWidgets(Guid rowId)
    {
        return GetWidgetPlacements().Any(e=>e.LayoutRowId == rowId);
    }

    public List<WidgetPlacement> GetWidgetPlacements()
    {
        var layout = Layouts.FirstOrDefault();
        return layout != null 
            ? layout.GetWidgetPlacements() 
            : new List<WidgetPlacement>();
    }
}