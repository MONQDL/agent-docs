namespace SystemMetricsPlugin.Models;

/// <summary>
/// Prometheus metric.
/// </summary>
public class PrometheusMetric
{
    /// <summary>
    /// Metric name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Metric labels.
    /// </summary>
    public ICollection<PrometheusLabel> Labels { get; set; } = [];

    /// <summary>
    /// Metric value.
    /// </summary>
    public double Sample { get; set; }

    /// <summary>
    /// Timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
