using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class LayoutType
{
    /// <summary>
    /// Primary Identifier (int)
    /// </summary>
    public int LayoutTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the Layout.
    /// Each column in the Layout is delimited with a slash ('/'). These columns should be defined by the CSS Framework using the Grid/Column system.
    /// For example, Tuxboard, by default, uses the Bootstrap Grid/Column system. The layout type titled "Three Columns, Equal" contains the following layout "col-4/col-4/col-4".
    /// Since the grid system is based on a 12-column system, each LayoutRow must equal 12.
    /// If you wanted a single layout type to use one large column, you could add a new layout type titled "One Column" with a layout of "col-12".
    /// Adjust these layout types based on the grid/column system for your preferred CSS frameworks.
    /// </summary>
    public string Layout { get; set; }

    /// <summary>
    /// Collection of LayoutRows using this Layout Type
    /// </summary>
    public virtual ICollection<LayoutRow> LayoutRows { get; set; } = new HashSet<LayoutRow>();
}