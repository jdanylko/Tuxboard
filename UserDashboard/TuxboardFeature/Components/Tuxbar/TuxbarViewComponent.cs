using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UserDashboard.TuxboardFeature.Components.Tuxbar
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
