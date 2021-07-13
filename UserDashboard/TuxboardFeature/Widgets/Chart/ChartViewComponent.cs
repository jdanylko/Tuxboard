using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.UI;

namespace UserDashboard.TuxboardFeature.Widgets.Chart
{
    [ViewComponent(Name="chart")]
    public class ChartViewComponent : ViewComponent
    {
        // EXTEND: Hook - Can use any Context for your application.
        // private readonly IMyDbContext _context;

        //public TableViewComponent(IMyDbContext context)
        //{
        //    _context = context;
        //}

        public ChartViewComponent() { }

        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            var model = new ChartModel
            {
                Placement = placement
            };

            return this.WidgetView(model);
        }
    }
}