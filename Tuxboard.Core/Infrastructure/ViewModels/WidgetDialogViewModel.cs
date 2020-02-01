using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels
{
    public class WidgetDialogViewModel
    {
        public IEnumerable<Widget> Widgets { get; set; }
        public List<string> Groups { get; set; }
    }
}
