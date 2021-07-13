using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;

namespace UserDashboard.TuxboardFeature.Components.LayoutTemplate
{
    public class LayoutTemplateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Layout layout)
        {
            return View(layout);
        }
    }
}
