using Microsoft.AspNetCore.Mvc;

namespace UserDashboard.TuxboardFeature.Components.LayoutRow
{
    [ViewComponent]
    public class LayoutRowViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(global::Tuxboard.Core.Domain.Entities.LayoutRow row)
        {
            return View(row);
        }
    }
}
