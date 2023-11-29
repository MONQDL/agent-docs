using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Exceptions;
using Monq.Plugins.Abstractions.Extensions;
using SystemInfoPlugin.HttpServices;
using SystemInfoPlugin.Models;

namespace SystemInfoPlugin;

/// <summary>
/// Plugin Task Execution Strategy.
/// </summary>
public class PluginTaskStrategy : IPluginTaskStrategy
{
    readonly IStreamDataCollectorApiHttpService _streamDataCollectorApiHttpService;

    /// <summary>
    /// Plugin Task Execution Strategy constructor.
    /// </summary>
    /// <param name="streamDataCollectorApiHttpService">Data Stream API service.</param>
    public PluginTaskStrategy(IStreamDataCollectorApiHttpService streamDataCollectorApiHttpService)
    {
        _streamDataCollectorApiHttpService = streamDataCollectorApiHttpService;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object?>> Run(IDictionary<string, object?> variables, IEnumerable<string> securedVariables, CancellationToken cancellationToken)
    {
        var config = variables.ToObject<TaskConfig>();
        ValidateConfig(config);
        var sysInfo = GetSystemInformation();
        await _streamDataCollectorApiHttpService.PushEvents(config.BaseUri, config.StreamKey, config.UserspaceId, new[] { sysInfo });
        return new Dictionary<string, object?>();
    }

    /// <summary>
    /// Perform validation of the <see cref="TaskConfig"/> class instance. If one of the config's properties 
    /// contains an invalid value or is empty the method throws <see cref="PluginNotConfiguredException"/>.
    /// </summary>
    /// <param name="config">Config to validate.</param>
    /// <exception cref="PluginNotConfiguredException"></exception>
    void ValidateConfig(TaskConfig config)
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
    SystemInformation GetSystemInformation()
    {
        return new SystemInformation()
        {
            Name = Environment.MachineName
        };
    }
}
