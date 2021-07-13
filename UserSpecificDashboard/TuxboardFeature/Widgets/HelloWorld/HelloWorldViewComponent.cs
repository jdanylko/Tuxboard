using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;
using Tuxboard.Core.UI;

namespace Tuxboard.UI.TuxboardFeature.Widgets.HelloWorld
{
    [ViewComponent(Name="helloworld")]
    public class HelloWorldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            var widgetModel = new WidgetModel {Placement = placement};
            
            return this.WidgetView(widgetModel);
        }
    }
}