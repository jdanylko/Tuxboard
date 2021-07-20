using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace UserDashboard.Pages
{
    public class TuxboardModel : PageModel
    {
        private readonly ILogger<TuxboardModel> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public Dashboard Dashboard { get; set; }

        public TuxboardModel(ILogger<TuxboardModel> logger,
            IDashboardService service,
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        public async Task<IActionResult> OnGet()
        {
            var userId = GetCurrentUserId();

            Dashboard dashboard;
            if (string.IsNullOrEmpty(userId))
            {
                dashboard = await _service.GetDashboardAsync(_config);
            }
            else
            {
                dashboard = await _service.GetDashboardForAsync(_config, userId);
            }

            if (dashboard == null)
            {
                return NotFound("Could not find dashboard.");
            }

            var tab = dashboard.Tabs.FirstOrDefault();

            return ViewComponent("LayoutTemplate", tab.Layouts.FirstOrDefault());
        }

        // Collapse Widget
        public async Task<IActionResult> OnPost([FromBody] WidgetParameter parms)
        {
            if (string.IsNullOrEmpty(parms.Id))
            {
                return NotFound("Placement id is null.");
            }

            var placement = await _service.GetWidgetPlacementAsync(parms.Id);
            if (placement == null)
            {
                return NotFound("Could not find widget.");
            }

            await _service.UpdateCollapsedAsync(parms.Id, parms.Collapsed == 1);

            return new OkObjectResult(null);
        }

        // Delete Widget
        public async Task<IActionResult> OnDelete([FromBody] DeleteWidgetParameter model)
        {
            var success = await _service.RemoveWidgetAsync(model.PlacementId);

            if (!success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Widget (id:{model.PlacementId}) was NOT removed.");
            }

            return new OkObjectResult(
                new TuxViewMessage("Widget was removed", TuxMessageType.Success, true, model.PlacementId));
        }

        // Save Widget location
        public async Task<IActionResult> OnPut([FromBody] PlacementParameter model)
        {
            var placement = await _service.SaveWidgetPlacementAsync(model);

            if (placement == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Widget Placement (id:{model.PlacementId}) was NOT saved.");
            }

            return new OkObjectResult("Widget Placement was saved.");
        }



        private string GetCurrentUserId()
        {
            if (User.Identity == null
                || string.IsNullOrEmpty(User.Identity.Name))
                return null;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;
        }
    }
}
