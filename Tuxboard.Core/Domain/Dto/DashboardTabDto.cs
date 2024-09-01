using System;
using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// Minimal instance of a <see cref="DashboardTab"/>.
/// </summary>
public class DashboardTabDto
{
    /// <summary>
    /// Get or set the dashboard tab Id
    /// </summary>
    public Guid TabId { get; set; }
    /// <summary>
    /// Get or set the dashboard tab title
    /// </summary>
    public string TabTitle { get; set; }
    /// <summary>
    /// Get or set the dashboard tab index
    /// </summary>
    public int TabIndex { get; set; }
    /// <summary>
    /// Get or set the list of <see cref="Layout"/>s.
    /// </summary>
    public List<LayoutDto> Layouts { get; set; } = new();
    /// <summary>
    /// Get or set the list of <see cref="WidgetPlacement"/>s.
    /// </summary>
    public List<WidgetPlacementDto> WidgetPlacements { get; set; } = new();
}