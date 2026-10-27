using Fennel.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Models;
using Monq.Plugins.Abstractions.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    public Task<JsonObject> Run(
        PluginTaskContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting system metrics...");

        var config = JsonSerializer.Deserialize(
            context.Variables,
            PluginJsonSerializerContext.Default.TaskConfig) ?? new();
        ValidateConfig(config);

        var customLabels = config.CustomFields
            .Select(kvp => new PrometheusLabel
            {
                Name = kvp.Key,
                Value = GetLabelValue(kvp.Value),
            })
            .ToList();
        var prometheusMetrics = GetSystemMetrics(customLabels);
        var prometheusTextMetrics = ConvertToPrometheusText(prometheusMetrics);

        var resultItems = new JsonArray();
        foreach (var metric in prometheusTextMetrics)
            resultItems.Add(metric);

        var result = new JsonObject
        {
            [ResultKey] = resultItems,
        };
        return Task.FromResult(result);
    }

    static void ValidateConfig(TaskConfig config)
    {
        if (config.CustomFields.Any(x => x.Value == null))
            throw new PluginNotConfiguredException();
    }

    static string GetLabelValue(JsonNode? value)
        => value is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var stringValue)
            ? stringValue
            : value?.ToJsonString() ?? string.Empty;

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
