using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Infrastructure.Interfaces;

namespace Tuxboard.UI.TuxboardControllers
{
    public class WidgetSettingsController : Controller
    {
        private readonly ILogger<WidgetSettingsController> _logger;
        private readonly IDashboardService _service;

        public WidgetSettingsController(ILogger<WidgetSettingsController> logger, 
            IDashboardService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("/WidgetSettings/{id}")]
        public async Task<IActionResult> WidgetSettings(string id)
        {
            // var userId = await GetCurrentUserAsync();

            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent("WidgetSettings", placement);
        }

        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}