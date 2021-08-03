using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;

namespace UserDashboard.TuxboardFeature.Components.Tuxboard
{
    [ViewComponent]
    public class TuxboardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string id, Dashboard model)
        {
            model.DashboardId = id;

            return View(model);
        }
    }
}