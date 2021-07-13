using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.UI;

namespace UserDashboard.TuxboardFeature.Widgets.GeneralInfo
{
    [ViewComponent(Name = "generalinfo")]
    public class GeneralInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            var infoViewModel = new GeneralInfoModel
            {
                Placement = placement,
                Percentage = 90,
                Icon = "fas fa-cogs fa-5x p-3"
            };
            
            return this.WidgetView(infoViewModel);
        }
    }
}