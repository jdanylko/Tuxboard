using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Dto
{
    public class DashboardTabDto
    {
        public string TabId { get; set; }
        public string TabTitle { get; set; }
        public int TabIndex { get; set; }
        public List<LayoutDto> Layouts { get; set; }
        public List<WidgetPlacementDto> WidgetPlacements { get; set; }
    }
}
