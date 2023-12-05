using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Models;
using SystemMetricsPlugin.HttpServices;
using SystemMetricsPlugin.HttpServices.Implementation;

namespace SystemMetricsPlugin;

/// <summary>
/// Plugin task bootstrap service. Intended to register <see cref="PluginTask"/> and dependencies in a DI container (<see cref="IServiceCollection"/>).
/// </summary>
public class PluginTaskBootstrap : IPluginTaskBootstrap
{
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    const string Name = "System Metrics Plugin";
    /// <summary>
    /// The name of the plugin's command.
    /// </summary>
    const string Command = "SystemMetricsPlugin";

    /// <inheritdoc/>
    public PluginTask PluginTask => new(Name, Command, typeof(PluginTaskStrategy));

    /// <inheritdoc/>
    public void RegisterServiceProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<PluginTaskStrategy>();
        services.AddScoped<IMetricsDataCollectorApiHttpService, MetricsDataCollectorApiHttpService>();
    }
}
