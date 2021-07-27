using System.Linq;
using System.Net;
using System.Security.Claims;
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
    public class LayoutDialogModel : PageModel
    {
        private readonly ILogger<LayoutDialogModel> _logger;
        private readonly IDashboardService _service;
        private readonly ITuxboardConfig _config;

        public Dashboard Dashboard { get; set; }

        public LayoutDialogModel(ILogger<LayoutDialogModel> logger,
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
            return ViewComponent("LayoutDialog", await GetLayoutDialogViewModelAsync(id));
        }

        public async Task<IActionResult> OnPostAddLayoutRow(string layoutTypeId)
        {
            var types = await _service.GetLayoutTypesAsync();

            var layoutRow = new LayoutRow
            {
                LayoutRowId = "0",
                LayoutTypeId = layoutTypeId,
                LayoutType = types.FirstOrDefault(e => e.LayoutTypeId == layoutTypeId)
            };

            return ViewComponent("LayoutDialog", layoutRow);
        }

        public async Task<IActionResult> OnPostSaveLayout([FromBody] SaveLayoutViewModel model)
        {
            var success = await _service.SaveLayoutAsync(model.TabId, model.LayoutList);
            if (!success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Layout (tabid:{model.TabId}) NOT saved.");
            }

            return new OkResult();
        }

        public async Task<IActionResult> OnPostDeleteLayoutRow([FromBody] PostData data)
        {
            var userId = GetCurrentUser();

            var id = data.LayoutRowId;

            TuxViewMessage message = null;

            var dashboard = await _service.GetDashboardForAsync(_config, userId);
            var layout = dashboard.GetLayoutByLayoutRow(id);

            var canDelete = true;

            if (layout.RowContainsWidgets(id))
            {
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

            return new OkObjectResult(message);
        }







        private async Task<LayoutDialogViewModel> GetLayoutDialogViewModelAsync(string tabId) =>
            new()
            {
                CurrentLayout = await _service.GetLayoutFromTabAsync(tabId),
                LayoutTypes = await _service.GetLayoutTypesAsync()
            };

        private string GetCurrentUser()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return null;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;

        }

    }

    public class PostData
    {
        public string Id { get; set; }
        public string LayoutRowId { get; set; }
        public string LayoutTypeId { get; set; }

    }
}
