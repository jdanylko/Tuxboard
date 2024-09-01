using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels;

/// <summary>
/// Create a model of current and available layout types for a Layout Dialog.
/// </summary>
public class LayoutDialogViewModel
{
    /// <summary>
    /// Get or set a collection of layout types
    /// </summary>
    public List<LayoutType> LayoutTypes { get; set; }

    /// <summary>
    /// Get or set the current layout
    /// </summary>
    public Layout CurrentLayout { get; set; }
}