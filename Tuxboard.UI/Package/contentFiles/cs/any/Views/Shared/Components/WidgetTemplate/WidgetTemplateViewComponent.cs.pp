using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;

namespace $rootnamespace$.Views.Shared.Components.WidgetTemplate
{
    public class WidgetTemplateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            return View(placement);
        }
    }
}

