using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class DashboardTab
    {
        public DashboardTab()
        {
            Layouts = new HashSet<Layout>();
        }

        public string TabId { get; set; }
        public string DashboardId { get; set; }
        public string TabTitle { get; set; }
        public int TabIndex { get; set; }

        public virtual Dashboard Dashboard { get; set; }
        public virtual ICollection<Layout> Layouts { get; set; }
    }
}
