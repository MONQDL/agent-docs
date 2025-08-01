using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Models;
using SystemInfoPlugin.HttpServices;
using SystemInfoPlugin.HttpServices.Implementation;

namespace SystemInfoPlugin;

/// <summary>
/// Plugin task bootstrap service. Intended to register <see cref="PluginTask"/> and dependencies in a DI container (<see cref="IServiceCollection"/>).
/// </summary>
public class PluginTaskBootstrap : IPluginTaskBootstrap
{
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    const string Name = "System Information Plugin";
    /// <summary>
    /// The name of the plugin's command.
    /// </summary>
    const string Command = "SystemInfoPlugin";

    /// <inheritdoc/>
    public PluginTask PluginTask => new(Name, Command, typeof(PluginTaskStrategy));

    /// <inheritdoc/>
    public void RegisterServiceProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<PluginTaskStrategy>();
        services.AddScoped<IStreamDataCollectorApiHttpService, StreamDataCollectorApiHttpService>();
    }
}
