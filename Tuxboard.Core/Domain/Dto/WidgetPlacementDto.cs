﻿namespace Tuxboard.Core.Domain.Dto;

public class WidgetPlacementDto
{
    public string WidgetPlacementId { get; set; }
    public string LayoutRowId { get; set; }
    public int ColumnIndex { get; set; }
    public int WidgetIndex { get; set; }
    public bool UseTemplate { get; set; }
    public bool UseSettings { get; set; }
    public bool Collapsed { get; set; }
    public bool Moveable { get; set; }
    public bool CanDelete { get; set; }

    public WidgetDto Widget { get; set; }
}