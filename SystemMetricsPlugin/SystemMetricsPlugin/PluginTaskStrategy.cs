using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using System.Text;
using SystemMetricsPlugin.HttpServices;
using SystemMetricsPlugin.Models;

namespace SystemMetricsPlugin;

/// <summary>
/// Plugin Task Execution Strategy.
/// </summary>
public class PluginTaskStrategy : IPluginTaskStrategy
{
    readonly IMetricsDataCollectorApiHttpService _metricDataCollectorApiHttpService;

    /// <summary>
    /// Plugin Task Execution Strategy constructor.
    /// </summary>
    /// <param name="metricDataCollectorApiHttpService">Metric Data Collector API service.</param>
    public PluginTaskStrategy(IMetricsDataCollectorApiHttpService metricDataCollectorApiHttpService)
    {
        _metricDataCollectorApiHttpService = metricDataCollectorApiHttpService;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object?>> Run(
        IDictionary<string, object?> variables,
        IEnumerable<string> securedVariables,
        CancellationToken cancellationToken)
    {
        var config = variables.ToObject<TaskConfig>();
        ValidateConfig(config);
        var sysMetrics = GetSystemMetrics();
        var metricsText = ConvertToPrometheusText(sysMetrics);
        await _metricDataCollectorApiHttpService.PushMetrics(config.BaseUri, config.StreamKey, config.UserspaceId, metricsText);
        return new Dictionary<string, object?>();
    }

    /// <summary>
    /// Perform validation of the <see cref="TaskConfig"/> class instance. If one of the config's properties 
    /// contains an invalid value or is empty the method throws <see cref="PluginNotConfiguredException"/>.
    /// </summary>
    /// <param name="config">Config to validate.</param>
    /// <exception cref="PluginNotConfiguredException"></exception>
    static void ValidateConfig(TaskConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.StreamKey)
            || string.IsNullOrWhiteSpace(config.BaseUri)
            || config.UserspaceId == 0)
            throw new PluginNotConfiguredException();
    }

    /// <summary>
    /// Returns the <see cref="SystemMetrics"/> object that contains the current system's metrics.
    /// </summary>
    static SystemMetrics GetSystemMetrics()
    {
        var drives = DriveInfo.GetDrives();
        var totalFreeSpace = drives.Sum(x => x.TotalFreeSpace);
        var totalSpace = drives.Sum(x => x.TotalSize);
        var diskSpaceUsagePercent = (1.0 - (double)totalFreeSpace / totalSpace) * 100;

        return new SystemMetrics()
        {
            MachineName = Environment.MachineName,
            DiskSpace = totalSpace,
            DiskSpaceUsagePercent = diskSpaceUsagePercent
        };
    }

    /// <summary>
    /// Converts the <see cref="SystemMetrics"/> object to Prometheus Text Based format.
    /// </summary>
    /// <param name="sysMetrics">system's metrics</param>
    static string ConvertToPrometheusText(SystemMetrics sysMetrics)
    {
        var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var sb = new StringBuilder();
        sb.AppendLine($@"disk_space{{machine_name=""{sysMetrics.MachineName}""}} {sysMetrics.DiskSpace} {timeStamp}");
        sb.AppendLine($@"disk_space_usage_percent{{machine_name=""{sysMetrics.MachineName}""}} {sysMetrics.DiskSpaceUsagePercent} {timeStamp}");
        return sb.ToString();
    }
}
