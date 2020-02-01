namespace Tuxboard.Core.Domain.Entities
{
    public partial class WidgetPlan
    {
        public string WidgetId { get; set; }
        public int PlanId { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual Widget Widget { get; set; }
    }
}
