using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;

namespace UserSpecificDashboard.TuxboardFeature.Tuxboard
{
    public class TuxboardController : Controller
    {
        private readonly ILogger<TuxboardController> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

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
            var userId = GetCurrentUser();

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
        [Route("Tuxboard/CollapseWidget")]
        public async Task<IActionResult> CollapseWidget([FromBody] WidgetParameter parms)
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

            await _service.UpdateCollapsedAsync(parms.Id, parms.Collapsed );

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