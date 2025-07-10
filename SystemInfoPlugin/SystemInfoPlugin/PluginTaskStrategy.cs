using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using Monq.Plugins.Abstractions.Services;
using System.Text.Json;
using SystemInfoPlugin.Models;

namespace SystemInfoPlugin;

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
    public Task<IDictionary<string, object?>> Run(IDictionary<string, object?> variables, IEnumerable<string> securedVariables, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting system info...");

        var config = variables.ToConfig<TaskConfig>();
        ValidateConfig(config);
        var sysInfo = GetSystemInformation();
        var result = new Dictionary<string, object?>()
        {
            [ResultKey] = JsonSerializer.Serialize(sysInfo),
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
    /// Returns the <see cref="SystemInformation"/> object that contains the current system's information.
    /// </summary>
    /// <returns></returns>
    static SystemInformation GetSystemInformation()
    {
        return new SystemInformation()
        {
            Name = Environment.MachineName
        };
    }
}
