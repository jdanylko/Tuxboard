using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace $rootnamespace$.TuxboardControllers
{
    public class TuxboardController : Controller
    {
        private readonly ILogger<TuxboardController> _logger;
        private readonly IDashboardService _service;
        private readonly TuxboardConfig _config = new TuxboardConfig();

        public TuxboardController(ILogger<TuxboardController> logger, 
            IDashboardService service, 
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        [HttpGet]
        [Route("Tuxboard/Get")]
        public async Task<IActionResult> Get()
        {
            var userId = await GetCurrentUserAsync();

            var dashboard = await _service.GetDashboardForAsync(_config, userId);
            if (dashboard == null)
            {
                return NotFound("Could not find dashboard.");
            }

            var tab = dashboard.Tabs.FirstOrDefault();
                        
            return ViewComponent("LayoutTemplate", tab.Layouts.FirstOrDefault());
        }

        [HttpPost]
        [Route("Tuxboard/PostCollapse")]
        public async Task<IActionResult> PostCollapse([FromBody] WidgetParameter parms)
        {
            // var user = await GetCurrentUserAsync();

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

            return Ok();
        }

        [HttpDelete]
        [Route("/Tuxboard/RemoveWidget/")]
        public async Task<IActionResult> RemoveWidget([FromBody] DeleteWidgetParameter model)
        {
            // var user = await GetCurrentUserAsync();

            var result = new TuxResponse { Success = true };

            var success = await _service.RemoveWidgetAsync(model.PlacementId);

            result.Message = new TuxViewMessage(
                success ? "Widget removed." : "Widget was NOT removed.",
                success ? TuxMessageType.Success : TuxMessageType.Danger,
                success, id: model.PlacementId);

            return Json(result);
        }

        [HttpPut]
        [Route("Tuxboard/Put")]
        public async Task<IActionResult> Put([FromBody] PlacementParameter model)
        {
            //var user = await GetCurrentUserAsync();

            var placement = await _service.SaveWidgetPlacementAsync(model);

            var result = new TuxResponse
            {
                Success = placement != null,
                Message = new TuxViewMessage(
                    placement != null ? "Widget placement saved." : "Widget placement NOT saved.",
                    placement != null ? TuxMessageType.Success : TuxMessageType.Danger)
            };

            return Ok(result);
        }



        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}
