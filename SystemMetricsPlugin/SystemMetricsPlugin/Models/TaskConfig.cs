using System.Text.Json.Nodes;

namespace SystemMetricsPlugin.Models;

/// <summary>
/// Task configuration.
/// </summary>
public sealed class TaskConfig
{
    /// <summary>
    /// Custom fields represented as Prometheus labels.
    /// </summary>
    public Dictionary<string, JsonNode?> CustomFields { get; init; } = [];
}
