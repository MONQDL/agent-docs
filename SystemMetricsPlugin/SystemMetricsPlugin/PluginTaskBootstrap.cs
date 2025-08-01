using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Models;

namespace SystemMetricsPlugin;

/// <summary>
/// Plugin task bootstrap service. Intended to register <see cref="PluginTask"/> and dependencies in a DI container (<see cref="IServiceCollection"/>).
/// </summary>
public class PluginTaskBootstrap : IPluginTaskBootstrap
{
    /// <summary>
    /// Plugin name.
    /// </summary>
    const string Name = "System Metrics Plugin";
    /// <summary>
    /// Plugin execution command.
    /// </summary>
    const string Command = "systemMetricsPlugin";

    /// <inheritdoc/>
    public PluginTask PluginTask => new(Name, Command, typeof(PluginTaskStrategy));

    /// <inheritdoc/>
    public void RegisterServiceProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<PluginTaskStrategy>();
    }
}
