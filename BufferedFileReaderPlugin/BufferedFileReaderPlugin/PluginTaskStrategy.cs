using BufferedFileReaderPlugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Models;
using Monq.Plugins.Abstractions.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BufferedFileReaderPlugin;

/// <summary>
/// Reads a text file and streams its lines to the agent.
/// </summary>
public sealed class PluginTaskStrategy : IPluginTaskStrategy
{
    readonly ILogger<PluginTaskStrategy> _logger;

    /// <summary>
    /// Plugin task execution strategy constructor.
    /// </summary>
    public PluginTaskStrategy(IProxyServiceProvider proxyServiceProvider)
    {
        _logger = proxyServiceProvider.GetRequiredService<ILogger<PluginTaskStrategy>>();
    }

    /// <inheritdoc/>
    public async Task<JsonObject> Run(
        PluginTaskContext context,
        CancellationToken cancellationToken)
    {
        var config = JsonSerializer.Deserialize(
            context.Variables,
            PluginJsonSerializerContext.Default.TaskConfig) ?? new();
        ValidateConfig(config);

        _logger.LogDebug("Reading file {FilePath}...", config.FilePath);

        await foreach (var line in File.ReadLinesAsync(config.FilePath, cancellationToken))
        {
            await context.WriteOutput(
                new JsonObject
                {
                    ["data"] = line,
                },
                cancellationToken);
        }

        return [];
    }

    static void ValidateConfig(TaskConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.FilePath))
            throw new PluginNotConfiguredException("File path is not defined.");

        if (!File.Exists(config.FilePath))
            throw new PluginNotConfiguredException($"File '{config.FilePath}' does not exist.");
    }
}
