using System.Text.Json.Nodes;

namespace SystemInfoPlugin.Models;

/// <summary>
/// Task configuration.
/// </summary>
public sealed class TaskConfig
{
    /// <summary>
    /// Custom fields.
    /// </summary>
    public Dictionary<string, JsonNode?> CustomFields { get; init; } = [];
}
