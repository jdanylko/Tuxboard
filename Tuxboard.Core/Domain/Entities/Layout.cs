using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class Layout
    {
        public Layout()
        {
            DashboardDefaults = new HashSet<DashboardDefault>();
            LayoutRows = new HashSet<LayoutRow>();
        }

        public string LayoutId { get; set; }
        public string TabId { get; set; }
        public int LayoutIndex { get; set; }

        public virtual DashboardTab Tab { get; set; }
        public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; }
        public virtual ICollection<LayoutRow> LayoutRows { get; set; }
    }
}
