using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutType
{
    public int LayoutTypeId { get; set; }
    public string Title { get; set; }
    public string Layout { get; set; }

    public virtual ICollection<LayoutRow> LayoutRows { get; set; } = new HashSet<LayoutRow>();
}