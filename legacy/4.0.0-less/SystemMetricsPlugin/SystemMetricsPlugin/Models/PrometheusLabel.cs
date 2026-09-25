namespace SystemMetricsPlugin.Models;

/// <summary>
/// Prometheus label.
/// </summary>
public class PrometheusLabel
{
    /// <summary>
    /// Label name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Label value.
    /// </summary>
    public required string Value { get; set; }
}
