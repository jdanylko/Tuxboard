using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserDashboard.TuxboardFeature.Components.WidgetDialog
{
    [ViewComponent]
    public class WidgetDialogViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(WidgetDialogViewModel dialog)
        {
            return View(dialog);
        }
    }
}
