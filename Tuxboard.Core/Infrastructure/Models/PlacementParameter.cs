using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Request class for moving a widget on a dashboard.
/// </summary>
public class PlacementParameter
{
    /// <summary>
    /// Get or set the widget placement id
    /// </summary>
    public Guid PlacementId { get; set; }
    /// <summary>
    /// Get or set the previous layout row ID
    /// </summary>
    public Guid PreviousLayoutRowId { get; set; }
    /// <summary>
    /// Get or set the previous column
    /// </summary>
    public int PreviousColumn { get; set; }
    /// <summary>
    /// Get or set the current layout row ID
    /// </summary>
    public Guid CurrentLayoutRowId { get; set; }
    /// <summary>
    /// Get or set the current column
    /// </summary>
    public int CurrentColumn { get; set; }
    /// <summary>
    /// Get or set a list of PlacementItems.
    /// </summary>
    public List<PlacementItem> PlacementList { get; set; }
}

/// <summary>
/// Class used to identify what widgets and what is their index position on a dashboard .
/// </summary>
public class PlacementItem
{
    /// <summary>
    /// Get or set the index of a widget on a dashboard.
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// Get or set the id of a widget placement on a dashboard.
    /// </summary>
    public Guid PlacementId { get; set; }
}