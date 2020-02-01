using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.ViewModels
{
    public class SaveSettingsViewModel
    {
        public List<WidgetSetting> Settings { get; set; }
    }
}