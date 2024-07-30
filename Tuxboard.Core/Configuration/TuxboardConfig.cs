namespace Tuxboard.Core.Configuration;

public class TuxboardConfig : ITuxboardConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public bool CreateSeedData { get; set; } = false;
}