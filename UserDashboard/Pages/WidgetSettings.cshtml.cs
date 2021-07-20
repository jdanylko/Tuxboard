using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserDashboard.Pages
{
    public class WidgetSettingsModel : PageModel
    {
        private readonly ILogger<WidgetSettingsModel> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public Dashboard Dashboard { get; set; }

        public WidgetSettingsModel(ILogger<WidgetSettingsModel> logger,
            IDashboardService service,
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        public async Task<IActionResult> OnGet(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent("WidgetSettings", placement);
        }

        public async Task<IActionResult> OnPostSave([FromBody] SaveSettingsViewModel model)
        {
            var result = await _service.SaveWidgetSettingsAsync(model.Settings);

            return new OkObjectResult(result);
        }
    }
}
