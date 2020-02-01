namespace Tuxboard.Core.Domain.Entities
{
    public partial class DashboardDefaultWidget
    {
        public string DefaultWidgetId { get; set; }
        public string DashboardDefaultId { get; set; }
        public string LayoutRowId { get; set; }
        public string WidgetId { get; set; }
        public int ColumnIndex { get; set; }
        public int WidgetIndex { get; set; }

        public virtual DashboardDefault DashboardDefault { get; set; }
        public virtual LayoutRow LayoutRow { get; set; }
        public virtual Widget Widget { get; set; }
    }
}
