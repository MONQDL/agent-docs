using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using Monq.Plugins.Abstractions.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using SystemInfoPlugin.Models;

namespace SystemInfoPlugin;

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
    public Task<IDictionary<string, object?>> Run(IDictionary<string, object?> variables, IEnumerable<string> securedVariables, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting system info...");

        var config = variables.ToConfig<TaskConfig>();
        ValidateConfig(config);

        var sysInfo = GetSystemInformation();

        var customFields = JsonSerializer.SerializeToNode(config.CustomFields)?.AsObject();
        var record = JsonSerializer.SerializeToNode(sysInfo)?.AsObject();
        SetCustomFields(record, customFields);

        var result = new Dictionary<string, object?>()
        {
            [ResultKey] = JsonSerializer.Serialize(record),
        };
        return Task.FromResult<IDictionary<string, object?>>(result);
    }

    static void ValidateConfig(TaskConfig config)
    {
        if (config.CustomFields.Any(x => x.Value == null))
            throw new PluginNotConfiguredException();
    }

    static SystemInformation GetSystemInformation()
    {
        return new SystemInformation()
        {
            Name = Environment.MachineName
        };
    }

    static void SetCustomFields(JsonObject? record, JsonObject? customFields)
    {
        if (record == null || customFields == null)
            return;

        foreach (var attr in customFields)
        {
            var value = attr.Value != null
                ? JsonNode.Parse(attr.Value.ToJsonString())
                : null;
            record.TryAdd(attr.Key, value);
        }
    }
}
