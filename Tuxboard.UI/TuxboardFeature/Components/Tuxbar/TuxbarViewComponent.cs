using Microsoft.AspNetCore.Mvc;

namespace Tuxboard.UI.TuxboardFeature.Components.Tuxbar
{
    public class TuxbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
