using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.UI.TuxboardFeature.Widgets.Table
{
    public class TableModel: WidgetModel
    {
        public List<Product> Products { get; set; }
    }
}