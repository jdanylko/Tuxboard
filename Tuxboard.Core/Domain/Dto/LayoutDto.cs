using System;
using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// DTO of a <see cref="Layout" />
/// </summary>
public class LayoutDto
{
    /// <summary>
    /// Get or set the LayoutID
    /// </summary>
    public Guid LayoutId { get; set; }
    /// <summary>
    /// Get or set the Layout Index
    /// </summary>
    public int LayoutIndex { get; set; }
    /// <summary>
    /// Get or set a List of Layout Rows.
    /// </summary>
    public List<LayoutRowDto> LayoutRows { get; set; }
}