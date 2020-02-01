using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace Tuxboard.UI.TuxboardControllers
{
    public class WidgetContentController : Controller
    {
        private readonly ILogger<WidgetContentController> _logger;
        private readonly IDashboardService _service;

        public WidgetContentController(ILogger<WidgetContentController> logger, 
            IDashboardService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("/Widget/{id}")]
        public async Task<IActionResult> Widget(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent(placement.Widget.Name, placement);
        }

        [HttpPost]
        [Route("/Widget/saveSettings/")]
        public async Task<IActionResult> WidgetSettings([FromBody] SaveSettingsViewModel model)
        {
            var result = await _service.SaveWidgetSettingsAsync(model.Settings);

            return Ok(result);
        }

    }
}