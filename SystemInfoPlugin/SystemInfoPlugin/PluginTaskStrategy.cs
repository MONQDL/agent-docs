using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Models;
using Monq.Plugins.Abstractions.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using SystemInfoPlugin.Models;
using SystemInfoPlugin.Services;

namespace SystemInfoPlugin;

/// <summary>
/// Plugin task execution strategy.
/// </summary>
public class PluginTaskStrategy : IPluginTaskStrategy
{
    const string NameKey = "name";
    const string ResultKey = "result";

    readonly ILogger<PluginTaskStrategy> _logger;
    readonly ISystemInformationProvider _systemInformationProvider;

    /// <summary>
    /// Plugin task execution strategy constructor.
    /// </summary>
    public PluginTaskStrategy(
        IProxyServiceProvider proxyServiceProvider,
        ISystemInformationProvider systemInformationProvider)
    {
        _logger = proxyServiceProvider.GetRequiredService<ILogger<PluginTaskStrategy>>();
        _systemInformationProvider = systemInformationProvider;
    }

    /// <inheritdoc/>
    public Task<JsonObject> Run(
        PluginTaskContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting system info...");

        var config = JsonSerializer.Deserialize(
            context.Variables,
            PluginJsonSerializerContext.Default.TaskConfig) ?? new();
        ValidateConfig(config);

        var record = GetSystemInformation();
        SetCustomFields(record, config.CustomFields);

        var result = new JsonObject
        {
            [ResultKey] = record.ToJsonString(),
        };
        return Task.FromResult(result);
    }

    static void ValidateConfig(TaskConfig config)
    {
        if (config.CustomFields.Any(x => x.Value == null))
            throw new PluginNotConfiguredException();
    }

    JsonObject GetSystemInformation()
    {
        return new JsonObject
        {
            [NameKey] = _systemInformationProvider.GetMachineName(),
        };
    }

    static void SetCustomFields(
        JsonObject record,
        IReadOnlyDictionary<string, JsonNode?> customFields)
    {
        foreach (var attr in customFields)
            record.TryAdd(attr.Key, attr.Value?.DeepClone());
    }
}
