using Microsoft.AspNetCore.Mvc;

namespace Tuxboard.UI.Views.Shared.Components.Tuxbar
{
    public class TuxbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
