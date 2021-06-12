﻿using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.UI.TuxboardFeature.Components.WidgetTemplate
{
    [ViewComponent]
    public class WidgetTemplateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            return View(placement);
        }
    }
}
