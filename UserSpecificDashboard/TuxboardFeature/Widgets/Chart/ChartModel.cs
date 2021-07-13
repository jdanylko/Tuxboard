using Tuxboard.Core.Infrastructure.Models;
using UserSpecificDashboard.TuxboardFeature.Widgets.Chart.ChartJsModel;

namespace UserSpecificDashboard.TuxboardFeature.Widgets.Chart
{
    public class ChartModel : WidgetModel
    {
        public ChartJs Chart { get; set; }
    }

}