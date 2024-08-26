namespace Tuxboard.Core.Configuration;

/// <inheritdoc/>
public class TuxboardConfig : ITuxboardConfig
{
    /// <inheritdoc/>
    public string ConnectionString { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Schema { get; set; } = "dbo";

    /// <inheritdoc/>
    public bool CreateSeedData { get; set; } = false;
}