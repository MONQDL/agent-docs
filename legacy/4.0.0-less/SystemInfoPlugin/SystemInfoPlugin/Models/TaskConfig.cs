namespace SystemInfoPlugin.Models;

/// <summary>
/// Task configuration.
/// </summary>
public class TaskConfig
{
    /// <summary>
    /// Custom fields.
    /// </summary>
    public Dictionary<string, object?> CustomFields { get; set; } = [];
}
