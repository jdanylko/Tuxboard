using System.Collections.Generic;
using Tuxboard.Core.Configuration;

namespace Tuxboard.Core.Domain.Dto;

public class DashboardDto
{
    public TuxboardConfigDto Settings { get; set; }
    public int SelectedTab { get; set; }
    public List<DashboardTabDto> Tabs { get; set; }
}