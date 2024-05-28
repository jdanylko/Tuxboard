using System.Collections.Generic;
using Tuxboard.Core.Infrastructure.Models;

namespace Tuxboard.Core.Infrastructure.ViewModels;

public class SaveLayoutViewModel
{
    public List<LayoutOrder> LayoutList { get; set; }

    public string TabId { get; set; }
}