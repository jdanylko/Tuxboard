using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;

namespace UserDashboard.Pages
{
    public class WidgetModel : PageModel
    {
        private readonly ILogger<WidgetModel> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public Dashboard Dashboard { get; set; }

        public WidgetModel(ILogger<WidgetModel> logger,
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

            return ViewComponent(placement.Widget.Name, placement);
        }

        public async Task<IActionResult> OnGetTemplate(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent("WidgetTemplate", placement);
        }
    }
}
