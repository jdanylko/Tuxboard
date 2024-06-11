using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels;

public class LayoutDialogViewModel
{
    public List<LayoutType> LayoutTypes { get; set; }

    public Layout CurrentLayout { get; set; }
}