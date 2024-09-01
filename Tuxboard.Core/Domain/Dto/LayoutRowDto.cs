using System;
using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// DTO of a Layout Row
/// </summary>
public class LayoutRowDto
{
    /// <summary>
    /// Get or set the Layout Row ID
    /// </summary>
    public Guid LayoutRowId { get; set; }
    /// <summary>
    /// Get or set the row index
    /// </summary>
    public int RowIndex { get; set; }
    /// <summary>
    /// Get or set a list of columns
    /// </summary>
    public List<Column> Columns { get; set; }
    /// <summary>
    /// Get or set the HtmlLayout
    /// </summary>
    public string HtmlLayout { get; internal set; }
}