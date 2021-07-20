using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserDashboard.TuxboardFeature.Components.LayoutColumn
{
    [ViewComponent]
    public class LayoutColumnViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LayoutDialogViewModel dialogModel)
        {
            return View(dialogModel);
        }
    }
}
