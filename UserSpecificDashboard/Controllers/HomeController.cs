using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserSpecificDashboard.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config = new TuxboardConfig();

        public HomeController(ILogger<HomeController> logger,
            IDashboardService service,
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUser();

            //ViewData["Title"] = "Home Page";

            var viewModel = new DashboardViewModel
            {
                Dashboard = await _service.GetDashboardForAsync(_config, user)
            };

            return View(viewModel);
        }

        [NonAction]
        private string GetCurrentUser()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return null;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;

        }
    }
}
