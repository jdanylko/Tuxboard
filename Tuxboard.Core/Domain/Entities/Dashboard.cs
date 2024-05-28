using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class Dashboard
{
    public Dashboard()
    {
            Tabs = new HashSet<DashboardTab>();
        }

    public string DashboardId { get; set; }
    public int SelectedTab { get; set; }
    public string UserId { get; set; }

    public virtual ICollection<DashboardTab> Tabs { get; set; }
}