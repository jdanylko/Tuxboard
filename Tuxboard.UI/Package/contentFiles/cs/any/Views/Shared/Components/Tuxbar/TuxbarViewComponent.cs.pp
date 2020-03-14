using Microsoft.AspNetCore.Mvc;

namespace $rootnamespace$.Views.Shared.Components.Tuxbar
{
    public class TuxbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

