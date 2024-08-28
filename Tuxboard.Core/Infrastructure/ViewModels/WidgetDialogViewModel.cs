using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels;

/// <summary>
/// ViewModel used for a viewing a collection of widgets
/// </summary>
public class WidgetDialogViewModel
{
    /// <summary>
    /// List of widgets available to users
    /// </summary>
    public IEnumerable<Widget> Widgets { get; set; }

    /// <summary>
    /// Distinct list of widget groups
    /// </summary>
    public List<string> Groups { get; set; }
}