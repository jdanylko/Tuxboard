namespace Tuxboard.Core.Configuration;

public interface ITuxboardConfig
{
    string ConnectionString { get; set; }
    string Schema { get; set; }
    bool CreateSeedData { get; set; }
}