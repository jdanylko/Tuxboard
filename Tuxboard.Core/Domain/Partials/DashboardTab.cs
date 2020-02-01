using System.Collections.Generic;
using System.Linq;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class DashboardTab
    {
        public DashboardTabDto ToDto()
        {
            return new DashboardTabDto
            {
                TabId = TabId,
                TabIndex = TabIndex,
                TabTitle = TabTitle,
                Layouts = Enumerable.Select<Layout, LayoutDto>(Layouts, layout => layout.ToDto())
                    .OrderBy(t => t.LayoutIndex)
                    .ToList(),
                WidgetPlacements = GetWidgetPlacements()
                    .Select<WidgetPlacement, WidgetPlacementDto>(wp => wp.ToDto())
                    .ToList()
            };
        }

        public List<Layout> GetLayouts()
        {
            return Enumerable.ToList<Layout>(Layouts);
        }

        public bool RowContainsWidgets(LayoutRow row)
        {
            return RowContainsWidgets((string) row.LayoutRowId);
        }

        public bool RowContainsWidgets(string rowId)
        {
            return GetWidgetPlacements().Any(e=>e.LayoutRowId == rowId);
        }

        public List<WidgetPlacement> GetWidgetPlacements()
        {
            var layout = Enumerable.FirstOrDefault<Layout>(Layouts);
            return layout != null 
                ? layout.GetWidgetPlacements() 
                : new List<WidgetPlacement>();
        }
    }
}
