using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities
{
    public partial class Plan
    {
        public Plan()
        {
            DashboardDefaults = new HashSet<DashboardDefault>();
            WidgetPlans = new HashSet<WidgetPlan>();
        }

        public int PlanId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; }
        public virtual ICollection<WidgetPlan> WidgetPlans { get; set; }
    }
}
