using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace Tuxboard.UI.TuxboardControllers
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

        #region View Component

        [HttpGet]
        [Route("/WidgetDialog/")]
        public async Task<IActionResult> Widget()
        {
            var viewModel = await GetWidgetDialogViewModelAsync();

            return PartialView("WidgetDialog", viewModel);
        }

        #endregion

        #region API

        [HttpPost]
        [Route("/WidgetDialog/AddWidget/")]
        public async Task<IActionResult> AddWidget([FromBody] AddWidgetParameter model)
        {
            var success = await _service.AddWidgetToTabAsync(model.TabId, model.WidgetId);
            if (!success)
            {
                return StatusCode((int)HttpStatusCode.ExpectationFailed,
                    $"Widget (id:{model.WidgetId}) NOT saved.");

            }

            return Ok();
        }

        #endregion
        
        private async Task<WidgetDialogViewModel> GetWidgetDialogViewModelAsync()
        {
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


        [NonAction]
        private async Task<string> GetCurrentUserAsync()
        {
            return await Task.FromResult(TuxConfiguration.DefaultUser);
        }

    }
}