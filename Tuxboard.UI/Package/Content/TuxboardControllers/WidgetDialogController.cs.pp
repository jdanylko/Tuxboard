using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace $rootnamespace$.TuxboardControllers
{
    public class WidgetDialogController : Controller
    {
        private readonly ILogger<WidgetDialogController> _logger;
        private IDashboardService _service;

        public WidgetDialogController(ILogger<WidgetDialogController> logger, 
            IDashboardService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("/WidgetDialog/")]
        public async Task<IActionResult> Widget()
        {
            var viewModel = await GetWidgetDialogViewModelAsync();

            return PartialView("WidgetDialog", viewModel);
        }

        [HttpPost]
        [Route("/WidgetDialog/AddWidget/")]
        public async Task<IActionResult> AddWidget([FromBody] AddWidgetParameter model)
        {
            var result = new TuxResponse { Success = true };

            var success = await _service.AddWidgetToTabAsync(model.TabId, model.WidgetId);

            result.Message = new TuxViewMessage(
                success ? "Widget added." : "Widget was NOT added.",
                success ? TuxMessageType.Success : TuxMessageType.Danger);

            return Json(result);
        }

        private async Task<WidgetDialogViewModel> GetWidgetDialogViewModelAsync()
        {
            var result = new WidgetDialogViewModel
            {
                Groups = null,
                Widgets = null
            };

            // var user = await GetCurrentUserAsync();

            // Parameter could be a planId or modified to pull widgets by roleId instead.
            var widgets = await _service.GetWidgetsForAsync();

            widgets.ForEach(widget =>
            {
                widget.GroupName = string.IsNullOrEmpty(widget.GroupName)
                    ? "General"
                    : widget.GroupName;
            });

            result.Groups = widgets.Select(y => y.GroupName).Distinct().ToList();
            result.Widgets = new List<Widget>(widgets);

            return result;
        }


        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}
