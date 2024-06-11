using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels;

public class DashboardViewModel
{
    public Dashboard Dashboard { get; set; }
    public IEnumerable<string> SearchPaths { get; set; }
}