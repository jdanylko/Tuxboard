using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Tuxboard.Core.UI
{
    public class TuxboardViewLocationExpander : IViewLocationExpander
    {
        private readonly string _widgetFolder;
        private readonly string _viewFolder;
        private readonly string _componentFolder;

        public TuxboardViewLocationExpander(
            string widgetFolder, 
            string viewFolder,
            string componentFolder)
        {
            _widgetFolder = widgetFolder;
            _viewFolder = viewFolder;
            _componentFolder = componentFolder;
        }

        public void PopulateValues(ViewLocationExpanderContext context) { }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Concat(
                new List<string>
                {
                    _widgetFolder, _viewFolder/*, _componentFolder*/
                }
            );
        }
    }
}
