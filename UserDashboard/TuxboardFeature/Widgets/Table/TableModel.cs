using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace UserDashboard.TuxboardFeature.Widgets.Table
{
    public class TableModel: WidgetModel
    {
        public List<Product> Products { get; set; }
    }
}