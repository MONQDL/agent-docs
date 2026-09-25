using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monq.Plugins.Abstractions;
using Monq.Plugins.Abstractions.Models;

namespace BufferedFileReaderPlugin;

/// <summary>
/// Registers the buffered file reader plugin task.
/// </summary>
public sealed class PluginTaskBootstrap : IPluginTaskBootstrap
{
    const string Name = "Buffered File Reader Plugin";
    const string Command = "bufferedFileReaderPlugin";

    /// <inheritdoc/>
    public PluginTask PluginTask { get; } = new(Name, Command, typeof(PluginTaskStrategy));

    /// <inheritdoc/>
    public void RegisterServiceProvider(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<PluginTaskStrategy>();
    }
}
