using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class LayoutRow
    {
        public LayoutRow()
        {
            DashboardDefaultWidgets = new HashSet<DashboardDefaultWidget>();
            WidgetPlacements = new HashSet<WidgetPlacement>();
        }

        public string LayoutRowId { get; set; }
        public string LayoutId { get; set; }
        public string LayoutTypeId { get; set; }
        public int RowIndex { get; set; }

        public virtual Layout Layout { get; set; }
        public virtual LayoutType LayoutType { get; set; }
        public virtual ICollection<DashboardDefaultWidget> DashboardDefaultWidgets { get; set; }
        public virtual ICollection<WidgetPlacement> WidgetPlacements { get; set; }
    }
}
