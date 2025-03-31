using System;
using System.ComponentModel.DataAnnotations;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
public partial class DashboardBase
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid DashboardId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int SelectedTab { get; set; }
}