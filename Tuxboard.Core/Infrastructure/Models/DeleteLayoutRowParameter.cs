using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Request parameter to remove a <see cref="LayoutRow"/> from a dashboard.
/// </summary>
public class DeleteLayoutRowParameter
{
    /// <summary>
    /// Get or set the layout row id to remove
    /// </summary>
    public string LayoutRowId { get; set; }

    /// <summary>
    /// Get or set the Tab Id containing the LayoutRowId; Used to confirm deleting a LayoutRow from a tab actually exists.
    /// </summary>
    public string TabId { get; set; }
}