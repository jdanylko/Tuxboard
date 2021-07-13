using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserDashboard.TuxboardFeature.WidgetSettings
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

        #region ViewComponent

        [HttpGet]
        [Route("/WidgetSettings/{id}")]
        public async Task<IActionResult> WidgetSettings(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent("WidgetSettings", placement);
        }

        #endregion

        #region API

        [HttpPost]
        [Route("/WidgetSettings/Save/")]
        public async Task<IActionResult> WidgetSettings([FromBody] SaveSettingsViewModel model)
        {
            var result = await _service.SaveWidgetSettingsAsync(model.Settings);

            return Ok(result);
        }

        #endregion

    }
}