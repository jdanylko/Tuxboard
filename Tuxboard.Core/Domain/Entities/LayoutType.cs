using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutType
{
    public LayoutType()
    {
            LayoutRows = new HashSet<LayoutRow>();
        }

    public string LayoutTypeId { get; set; }
    public string Title { get; set; }
    public string Layout { get; set; }

    public virtual ICollection<LayoutRow> LayoutRows { get; set; }
}