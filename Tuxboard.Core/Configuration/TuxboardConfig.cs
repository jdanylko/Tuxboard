namespace Tuxboard.Core.Configuration;

public class TuxboardConfig : ITuxboardConfig
{
    public string ConnectionString { get; set; }
    public string Schema { get; set; }
    public string WidgetFolder { get; set; }
    public string ViewFolder { get; set; }
    public string ComponentFolder { get; set; }
}