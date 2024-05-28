namespace Tuxboard.Core.Domain.Dto;

public class WidgetDto
{
    public string WidgetId { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string GroupName { get; set; }
    public int Permission { get; set; }
}