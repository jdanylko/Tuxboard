using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Tuxboard.Core.Domain.Entities;

namespace UserSpecificDashboard.TuxboardFeature.Components.WidgetSettings
{
    [HtmlTargetElement("widget-settings")]
    public class WidgetSettingsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            return View(placement);
        }
    }
}