using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.UI;

namespace Tuxboard.UI.Widgets.Table
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