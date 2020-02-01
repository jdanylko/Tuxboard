using System.Collections.Generic;

namespace Tuxboard.Core.Infrastructure.Models
{
    public class PlacementParameter
    {
        public int PreviousColumn { get; set; }
        public string PreviousLayout { get; set; }
        public string PlacementId { get; set; }
        public string LayoutRowId { get; set; }
        public int Column { get; set; }
        public List<PlacementItem> PlacementList { get; set; }
    }

    public class PlacementItem
    {
        public int Index { get; set; }
        public string PlacementId { get; set; }
    }
}