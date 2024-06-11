using System;

namespace Tuxboard.Core.Infrastructure.Models;

public class AddWidgetResponse
{
    public bool Success { get; set; }
    public Guid PlacementId { get; set; }
}