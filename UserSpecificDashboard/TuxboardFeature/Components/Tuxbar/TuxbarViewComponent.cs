using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UserSpecificDashboard.TuxboardFeature.Components.Tuxbar
{
    [HtmlTargetElement("tuxbar")]
    public class TuxbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
