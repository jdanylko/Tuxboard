using System.Collections.Generic;
using Tuxboard.Core.Configuration;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Dto;

/// <summary>
/// minimal DashboardDto (data transfer objects)
/// </summary>
public class DashboardDto
{
    /// <summary>
    /// TuxboardConfigDto
    /// </summary>
    public TuxboardConfigDto Settings { get; set; }
    /// <summary>
    /// Index of the currently selected <see cref="DashboardTab" /> (defaulted to 1).
    /// </summary>
    public int SelectedTab { get; set; }
    /// <summary>
    /// DashboardTabDto for DashboardTabs
    /// </summary>
    public List<DashboardTabDto> Tabs { get; set; }
}