using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Infrastructure.Models;

public class PlacementParameter
{
    public Guid PlacementId { get; set; }
    public Guid PreviousLayoutRowId { get; set; }
    public int PreviousColumn { get; set; }
    public Guid CurrentLayoutRowId { get; set; }
    public int CurrentColumn { get; set; }
    public List<PlacementItem> PlacementList { get; set; }
}

public class PlacementItem
{
    public int Index { get; set; }
    public Guid PlacementId { get; set; }
}