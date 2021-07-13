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
using Tuxboard.Core.Infrastructure.ViewModels;

namespace Tuxboard.UI.TuxboardFeature.LayoutDialog
{
    public class LayoutDialogController : Controller
    {
        private readonly ILogger<LayoutDialogController> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public LayoutDialogController(ILogger<LayoutDialogController> logger, 
            IDashboardService service, 
            IOptions<TuxboardConfig> config)
        {
            _logger = logger;
            _service = service;
            _config = config.Value;
        }

        #region Partial Views

        [HttpPost]
        [Route("/LayoutDialog/AddLayoutRow/{layoutTypeId}")]
        public async Task<IActionResult> AddLayoutRow(string layoutTypeId)
        {
            var types = await _service.GetLayoutTypesAsync();

            var layoutRow = new LayoutRow
            {
                LayoutRowId = "0",
                LayoutTypeId = layoutTypeId,
                LayoutType = types.FirstOrDefault(e => e.LayoutTypeId == layoutTypeId)
            };

            return PartialView("LayoutRow", layoutRow);
        }

        [HttpPost]
        [Route("/LayoutDialog/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            return PartialView("LayoutDialog", await GetLayoutDialogViewModelAsync(id));
        }

        #endregion

        #region API

        [HttpPost]
        [Route("/LayoutDialog/SaveLayout/")]
        public async Task<IActionResult> SaveLayout([FromBody] SaveLayoutViewModel model)
        {
            var success = await _service.SaveLayoutAsync(model.TabId, model.LayoutList);
            if (!success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Layout (tabid:{model.TabId}) NOT saved.");
            }

            return Ok();
        }

        [HttpDelete]
        [Route("/LayoutDialog/DeleteLayoutRow/{id}")]
        public IActionResult DeleteLayoutRow(string id)
        {
            var userId = GetCurrentUser();

            TuxViewMessage message = null;

            var dashboard = _service.GetDashboardFor(_config, userId);
            var layout = dashboard.GetLayoutByLayoutRow(id);

            var canDelete = true;

            if (layout.RowContainsWidgets(id)) {
                message = new TuxViewMessage(
                    "Row contains widgets and cannot be deleted.",
                    TuxMessageType.Danger, false, id);
                canDelete = false;
            }

            var oneRowExists = layout.ContainsOneRow();
            if (oneRowExists && message == null)
            {
                message = new TuxViewMessage(
                    "You cannot delete the only row on the dashboard.",
                    TuxMessageType.Danger, false);
                canDelete = false;
            }

            if (canDelete)
            {
                message = new TuxViewMessage(
                    "Row can be removed.",
                    TuxMessageType.Success, true);
            }

            return Ok(message);
        }

        #endregion

        private async Task<LayoutDialogViewModel> GetLayoutDialogViewModelAsync(string tabId)
        {
            return new LayoutDialogViewModel
            {
                CurrentLayout = await _service.GetLayoutFromTabAsync(tabId),
                LayoutTypes = await _service.GetLayoutTypesAsync()
            };
        }

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