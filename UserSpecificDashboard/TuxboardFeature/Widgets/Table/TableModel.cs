using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace UserSpecificDashboard.TuxboardFeature.Widgets.Table
{
    public class TableModel: WidgetModel
    {
        public List<Product> Products { get; set; }
    }
}