using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Interfaces;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.Infrastructure.ViewModels;
using Tuxboard.Core.UI;

namespace Tuxboard.UI.TuxboardFeature.WidgetDialog
{
    public class WidgetDialogController : Controller
    {
        private readonly ILogger<WidgetDialogController> _logger;
        private IDashboardService _service;
        private readonly IServiceProvider _provider;

        public WidgetDialogController(ILogger<WidgetDialogController> logger, 
            IDashboardService service,
            IServiceProvider provider)
        {
            _logger = logger;
            _service = service;
            _provider = provider;
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

            return Ok(new
            {
                response.PlacementId,
                response.Success,
                Template = viewTemplate
            });
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

    }
}