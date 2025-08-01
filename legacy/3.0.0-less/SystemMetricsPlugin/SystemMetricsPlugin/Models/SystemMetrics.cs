namespace SystemMetricsPlugin.Models;
/// <summary>
/// Represents a system's information and metrics.
/// </summary>
public class SystemMetrics
{
    /// <summary>
    /// Represents a name of a system.
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Represents the total size of storage space on all drives in bytes.
    /// </summary>
    public double DiskSpace { get; set; }

    /// <summary>
    /// Represents the ratio of the total free space and the total size of storage space on all drives in percent.
    /// </summary>
    public double DiskSpaceUsagePercent { get; set; }
}
