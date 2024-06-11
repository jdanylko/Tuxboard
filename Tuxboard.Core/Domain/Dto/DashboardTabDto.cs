using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Dto;

public class DashboardTabDto
{
    public Guid TabId { get; set; }
    public string TabTitle { get; set; }
    public int TabIndex { get; set; }
    public List<LayoutDto> Layouts { get; set; } = new();
    public List<WidgetPlacementDto> WidgetPlacements { get; set; } = new();
}