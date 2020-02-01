using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.UI.Widgets.Table
{
    public class TableModel: WidgetModel
    {
        public List<Product> Products { get; set; }
    }
}