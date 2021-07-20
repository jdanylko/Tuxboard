using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Infrastructure.ViewModels;

namespace UserDashboard.TuxboardFeature.Components.LayoutDialog
{
    [ViewComponent]
    public class LayoutDialogViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LayoutDialogViewModel dialog)
        {
            return View(dialog);
        }
    }
}
