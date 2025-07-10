using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using Monq.Plugins.Abstractions.Services;
using SystemMetricsPlugin.Models;

namespace SystemMetricsPlugin;

/// <summary>
/// Plugin Task Execution Strategy.
/// </summary>
public class PluginTaskStrategy : IPluginTaskStrategy
{
    const string ResultKey = "result";

    readonly ILogger<PluginTaskStrategy> _logger;

    /// <summary>
    /// Plugin Task Execution Strategy constructor.
    /// </summary>
    public PluginTaskStrategy(
        IProxyServiceProvider proxyServiceProvider)
    {
        _logger = proxyServiceProvider.GetRequiredService<ILogger<PluginTaskStrategy>>();
    }

    /// <inheritdoc/>
    public Task<IDictionary<string, object?>> Run(
        IDictionary<string, object?> variables,
        IEnumerable<string> securedVariables,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting system metrics...");

        var config = variables.ToConfig<TaskConfig>();
        ValidateConfig(config);
        var sysMetrics = GetSystemMetrics();
        var prometheusMetrics = ConvertToPrometheusText(sysMetrics);
        var result = new Dictionary<string, object?>()
        {
            [ResultKey] = prometheusMetrics,
        };
        return Task.FromResult<IDictionary<string, object?>>(result);
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
    static IEnumerable<string> ConvertToPrometheusText(SystemMetrics sysMetrics)
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        return
        [
            $@"disk_space{{machine_name=""{sysMetrics.MachineName}""}} {sysMetrics.DiskSpace} {timestamp}",
            $@"disk_space_usage_percent{{machine_name=""{sysMetrics.MachineName}""}} {sysMetrics.DiskSpaceUsagePercent} {timestamp}"
        ];
    }
}
