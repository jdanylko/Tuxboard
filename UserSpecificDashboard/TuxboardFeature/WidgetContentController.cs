﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tuxboard.Core.Infrastructure.Interfaces;

namespace UserSpecificDashboard.TuxboardFeature
{
    public class WidgetContentController : Controller
    {
        private readonly ILogger<WidgetContentController> _logger;
        private readonly IDashboardService _service;

        public WidgetContentController(ILogger<WidgetContentController> logger, 
            IDashboardService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("/Widget/{id}")]
        public async Task<IActionResult> Widget(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent(placement.Widget.Name, placement);
        }

        [HttpGet]
        [Route("/WidgetTemplate/{id}")]
        public async Task<IActionResult> WidgetTemplate(string id)
        {
            var placement = await _service.GetWidgetPlacementAsync(id);

            return ViewComponent("WidgetTemplate", placement);
        }

    }
}