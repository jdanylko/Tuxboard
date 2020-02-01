using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.UI
{
    public static class ViewComponentExtensions
    {
        public static ViewViewComponentResult WidgetView(this ViewComponent component, WidgetModel model)
        {
            var viewName = $"{TuxConfiguration.WidgetDefaultPath}{model.Placement.Widget.Name}/{TuxConfiguration.WidgetDefaultFileName}";
            return component.View(viewName, model);
        }
    }
}