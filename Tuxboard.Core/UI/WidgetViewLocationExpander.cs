using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Tuxboard.Core.UI
{
    public class WidgetViewLocationExpander : IViewLocationExpander
    {
        private IEnumerable<string> _locations = new List<string>
        {
            $"~/Widgets/{{0}}.cshtml"
        };

        public void PopulateValues(ViewLocationExpanderContext context) { }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Concat(_locations);
        }
    }
}
