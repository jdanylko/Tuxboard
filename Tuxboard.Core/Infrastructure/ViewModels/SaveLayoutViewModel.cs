using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.ViewModels;

/// <summary>
/// Creates a "snapshot" of a layout received from the client to reconcile the layout rows;
/// Used on the Advanced Layout dialog when moving layout rows up and down.
/// </summary>
public class SaveLayoutViewModel
{
    /// <summary>
    /// The list of layout rows, their order, and their types
    /// </summary>
    public List<LayoutOrder> LayoutList { get; set; }

    /// <summary>
    /// a <see cref="DashboardTab"/> Id
    /// </summary>
    public string TabId { get; set; }
}