using Microsoft.AspNetCore.Mvc;

namespace Tuxboard.UI.Controllers
{
    public class DocsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
