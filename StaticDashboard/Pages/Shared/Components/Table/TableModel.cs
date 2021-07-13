using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace StaticDashboard.Pages.Shared.Components.Table
{
    public class TableModel: WidgetModel
    {
        public List<Product> Products { get; set; }
    }
}