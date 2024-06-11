using System;

namespace Tuxboard.Core.Domain.Dto;

public class WidgetSettingDto
{
    public string Name { get; set; }
    public string Value { get; set; }
    public Guid Id { get; set; }
}