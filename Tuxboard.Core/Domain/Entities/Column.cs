namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// Create a Column for identifying a <see cref="LayoutType"/>.
/// </summary>
public class Column
{
    /// <summary>
    /// Defines the order of the columns in a <see cref="LayoutRow"/> (sorted on render)
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Returns a CSS class defined in the layout type table delimited by slashes ('/')
    /// </summary>
    public string ColumnClass { get; set; }
}