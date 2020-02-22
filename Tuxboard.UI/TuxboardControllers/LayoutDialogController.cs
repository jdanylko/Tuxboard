using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.ViewModels;
using Tuxboard.UI.Controllers;

namespace Tuxboard.UI.TuxboardControllers
{
    public class LayoutDialogController : Controller
    {
        private readonly ILogger<LayoutDialogController> _logger;
        private readonly IDashboardService _service;
        private readonly TuxboardConfig _config = new TuxboardConfig();

        public LayoutDialogController(ILogger<LayoutDialogController> logger, 
            IDashboardService service, 
            IConfiguration config)
        {
            _logger = logger;
            _service = service;
            config
                .GetSection(nameof(TuxboardConfig))
                .Bind(_config);
        }

        [HttpPost]
        [Route("/LayoutDialog/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var viewModel = await GetLayoutDialogViewModelAsync(id);

            return PartialView("LayoutDialog", viewModel);
        }

        [HttpPost]
        [Route("/LayoutDialog/SaveLayout/")]
        public async Task<IActionResult> SaveLayout([FromBody] SaveLayoutViewModel model)
        {
            var result = new TuxResponse { Success = true };

            var layout = await _service.GetLayoutFromTabAsync(model.TabId);
            var success = await _service.SaveLayoutAsync(layout, model.LayoutList);

            result.Message = new TuxViewMessage(
                success ? "Layout saved." : "Layout NOT saved.",
                success ? TuxMessageType.Success : TuxMessageType.Danger,
                success);

            return Json(result);
        }

        [HttpDelete]
        [Route("/LayoutDialog/DeleteLayoutRow/{id}")]
        public IActionResult DeleteLayoutRow(string id)
        {
            var userId = TuxConfiguration.DefaultUser;

            TuxViewMessage message = null;

            var dashboard = _service.GetDashboardFor(userId);
            var layout = dashboard.GetLayoutByLayoutRow(id);

            var canDelete = true;

            var row = layout.LayoutRows.FirstOrDefault(t => t.LayoutRowId == id);
            if (row != null)
            {
                var widgetsExist = row.RowContainsWidgets();
                if (widgetsExist)
                {
                    message = new TuxViewMessage(
                        "Row contains widgets and cannot be deleted.",
                        TuxMessageType.Danger, false, row.LayoutRowId);
                    canDelete = false;
                }
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

        [HttpPost]
        [Route("/LayoutDialog/AddLayoutRow/{layoutTypeId}")]
        public async Task<IActionResult> AddLayoutRow(string layoutTypeId)
        {
            var types = await _service.GetLayoutTypesAsync();
            var rowType = types.FirstOrDefault(e => e.LayoutTypeId == layoutTypeId);
            var layoutRow = new LayoutRow
            {
                LayoutRowId = "0",
                LayoutTypeId = layoutTypeId,
                LayoutType = rowType
            };

            return PartialView("LayoutRow", layoutRow);
        }

        private async Task<LayoutDialogViewModel> GetLayoutDialogViewModelAsync(string tabId)
        {
            var layout = await _service.GetLayoutFromTabAsync(tabId);
            var types = await _service.GetLayoutTypesAsync();

            return new LayoutDialogViewModel
            {
                CurrentLayout = layout,
                LayoutTypes = types
            };
        }
    }
}