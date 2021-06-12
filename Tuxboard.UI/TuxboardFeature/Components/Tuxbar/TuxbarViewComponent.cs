using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Tuxboard.UI.TuxboardFeature.Components.Tuxbar
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
