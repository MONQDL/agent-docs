using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Models;
using SystemInfoPlugin.Services;

namespace SystemInfoPlugin;

/// <summary>
/// Plugin task bootstrap service. Intended to register <see cref="PluginTask"/> and dependencies in a DI container (<see cref="IServiceCollection"/>).
/// </summary>
public class PluginTaskBootstrap : IPluginTaskBootstrap
{
    /// <summary>
    /// Plugin name.
    /// </summary>
    const string Name = "System Information Plugin";
    /// <summary>
    /// Plugin execution command.
    /// </summary>
    const string Command = "systemInfo";

    /// <inheritdoc/>
    public PluginTask PluginTask => new(Name, Command, typeof(PluginTaskStrategy));

    /// <inheritdoc/>
    public void RegisterServiceProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISystemInformationProvider, SystemInformationProvider>();
        services.AddTransient<PluginTaskStrategy>();
    }
}
