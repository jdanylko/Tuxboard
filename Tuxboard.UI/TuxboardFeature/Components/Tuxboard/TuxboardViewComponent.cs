using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.UI.TuxboardFeature.Components.Tuxboard
{
    [ViewComponent]
    public class TuxboardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string id, Dashboard model)
        {
            model.DashboardId = id;

            return View(model);
        }
    }
}