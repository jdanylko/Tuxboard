using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.UI.TuxboardControllers
{
    public class TuxboardController : Controller
    {
        private readonly ILogger<TuxboardController> _logger;
        private readonly IDashboardService _service;
        private readonly TuxboardConfig _config;

        public TuxboardController(ILogger<TuxboardController> logger, 
            IDashboardService service, 
            IOptions<TuxboardConfig> config)
        {
            _logger = logger;
            _service = service;
            _config = config.Value;
        }

        #region ViewComponents

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

        #endregion

        #region API

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

            var success = await _service.RemoveWidgetAsync(model.PlacementId);

            if (!success)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    $"Widget (id:{model.PlacementId}) was NOT removed.");
            }

            return Ok(
                new TuxViewMessage("Widget was removed", TuxMessageType.Success, true, model.PlacementId));
        }

        [HttpPut]
        [Route("Tuxboard/Put")]
        public async Task<IActionResult> Put([FromBody] PlacementParameter model)
        {
            //var user = await GetCurrentUserAsync();

            var placement = await _service.SaveWidgetPlacementAsync(model);

            if (placement == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Widget Placement (id:{model.PlacementId}) was NOT saved.");
            }

            return Ok("Widget Placement was saved.");
        }

        #endregion

        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}