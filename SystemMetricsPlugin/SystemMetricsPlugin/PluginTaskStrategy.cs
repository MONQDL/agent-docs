using Fennel.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using Monq.Plugins.Abstractions.Services;
using SystemMetricsPlugin.Models;

namespace SystemMetricsPlugin;

/// <summary>
/// Plugin task execution strategy.
/// </summary>
public class PluginTaskStrategy : IPluginTaskStrategy
{
    const string ResultKey = "result";

    readonly ILogger<PluginTaskStrategy> _logger;

    /// <summary>
    /// Plugin task execution strategy constructor.
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

        var customLabels = config.CustomFields
            .Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value?.ToString() ?? string.Empty))
            .Select(kvp => new PrometheusLabel
            {
                Name = kvp.Key,
                Value = kvp.Value,
            })
            .ToList();
        var prometheusMetrics = GetSystemMetrics(customLabels);
        var prometheusTextMetrics = ConvertToPrometheusText(prometheusMetrics);

        var result = new Dictionary<string, object?>()
        {
            [ResultKey] = prometheusTextMetrics,
        };
        return Task.FromResult<IDictionary<string, object?>>(result);
    }

    static void ValidateConfig(TaskConfig config)
    {
        if (config.CustomFields.Any(x => x.Value == null))
            throw new PluginNotConfiguredException();
    }

    static List<PrometheusMetric> GetSystemMetrics(IEnumerable<PrometheusLabel> customLabels)
    {
        var drives = DriveInfo.GetDrives();
        var totalFreeSpace = drives.Sum(x => x.TotalFreeSpace);
        var totalSpace = drives.Sum(x => x.TotalSize);
        var diskSpaceUsagePercent = (1.0 - (double)totalFreeSpace / totalSpace) * 100;
        var machineName = Environment.MachineName;
        var timestamp = DateTimeOffset.Now;

        var result = new List<PrometheusMetric>
        {
            new()
            {
                Name = "disk_space",
                Labels = [new() { Name = "machine_name", Value = machineName }],
                Sample = totalSpace,
                Timestamp = timestamp,
            },
            new()
            {
                Name = "disk_space_usage_percent",
                Labels = [new() { Name = "machine_name", Value = machineName }],
                Sample = diskSpaceUsagePercent,
                Timestamp = timestamp,
            },
        };

        foreach (var label in customLabels)
            foreach (var metric in result)
                metric.Labels.Add(label);
        return result;
    }

    static List<string> ConvertToPrometheusText(IEnumerable<PrometheusMetric> metrics)
    {
        var result = new List<string>();
        foreach (var metric in metrics)
        {
            var prometheusString = Prometheus.Metric(
                metric.Name,
                metric.Sample,
                metric.Labels.ToDictionary(l => l.Name, l => l.Value),
                metric.Timestamp);
            result.Add(prometheusString);
        }
        return result;
    }
}
