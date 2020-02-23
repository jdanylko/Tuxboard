using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.UI.Views.Shared.Components.WidgetSettings
{
    public class WidgetSettingsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            return View(placement);
        }
    }
}