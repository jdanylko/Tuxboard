namespace Tuxboard.Core.Configuration
{
    public interface ITuxboardConfig
    {
        string ConnectionString { get; set; }
        string DefaultUser { get; set; }
        string WidgetFolder { get; set; }
        string ViewFolder { get; set; }
        string ComponentFolder { get; set; }
    }
}