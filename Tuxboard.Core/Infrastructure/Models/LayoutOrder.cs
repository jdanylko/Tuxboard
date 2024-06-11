using System;

namespace Tuxboard.Core.Infrastructure.Models;

public class LayoutOrder
{ 
    public int Index { get; set; }
    public Guid? LayoutRowId { get; set; }
    public int TypeId { get; set; }
}