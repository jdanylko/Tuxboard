namespace Tuxboard.Core.Configuration;

/// <summary>
/// Represents the configuration for Tuxboard.
/// </summary>
public interface ITuxboardConfig
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the migration assembly.
    /// </summary>
    string MigrationAssembly { get; set; }

    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    string Schema { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to create seed data.
    /// </summary>
    bool CreateSeedData { get; set; }
}