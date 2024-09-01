using System;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.Models;

/// <summary>
/// Creates an item identifying a <see cref="LayoutRow"/>, the type of row,
/// and the index for ordering the layout rows.
/// </summary>
public class LayoutOrder
{ 
    /// <summary>
    /// The order of the layout row
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// The ID of the layout row
    /// </summary>
    public Guid? LayoutRowId { get; set; }
    /// <summary>
    /// The layout row's type (refer to the layout type table in the database)
    /// </summary>
    public int TypeId { get; set; }
}