using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.ViewModels;
using Tuxboard.Core.UI;

namespace UserDashboard.Pages
{
    public class WidgetDialogModel : PageModel
    {
        private readonly ILogger<WidgetDialogModel> _logger;
        private readonly IDashboardService _service;
        private readonly IServiceProvider _provider;

        public Dashboard Dashboard { get; set; }

        public WidgetDialogModel(ILogger<WidgetDialogModel> logger,
            IDashboardService service,
            IServiceProvider provider)
        {
            _logger = logger;
            _service = service;
            _provider = provider;
        }

        public async Task<IActionResult> OnGet()
        {
            var viewModel = await GetWidgetDialogViewModelAsync();

            return ViewComponent("WidgetDialog", viewModel);
        }

        public async Task<IActionResult> OnPostAddWidget([FromBody] AddWidgetParameter model)
        {
            var response = await _service.AddWidgetToTabAsync(model.TabId, model.WidgetId);
            if (!response.Success)
            {
                return StatusCode((int)HttpStatusCode.ExpectationFailed,
                    $"Widget (id:{model.WidgetId}) NOT saved.");
            }

            var widgetPlacement = await _service.GetWidgetPlacementAsync(response.PlacementId);

            var helper = new ViewRenderHelper(_provider);
            var viewTemplate = helper.RenderToString("WidgetTemplate",
                widgetPlacement, "Components/WidgetTemplate/Default");

            return new OkObjectResult(new
            {
                response.PlacementId,
                response.Success,
                Template = viewTemplate
            });
        }

        private async Task<WidgetDialogViewModel> GetWidgetDialogViewModelAsync()
        {
            // Get the PlanId for the user (WidgetPlan <-> Plan)
            // to retrieve specific widgets for a user based on a plan.
            // Integrate the PlanId into the IdentityUser as an extra property.
            // var planId = 1 (default is 0).

            var widgets = await _service.GetWidgetsForAsync();

            widgets.ForEach(widget =>
            {
                widget.GroupName = string.IsNullOrEmpty(widget.GroupName)
                    ? "General"
                    : widget.GroupName;
            });

            var result = new WidgetDialogViewModel
            {
                Groups = widgets.Select(y => y.GroupName).Distinct().ToList(),
                Widgets = new List<Widget>(widgets)
            };

            return result;
        }
    }
}
