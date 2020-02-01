using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace Tuxboard.UI.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _service;
        private TuxboardConfig _config = new TuxboardConfig();

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
            var user = await GetCurrentUserAsync();

            var viewModel = new DashboardViewModel
            {
                Dashboard = await _service.GetDashboardForAsync(_config, user)
            };

            return View(viewModel);
        }

        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}
