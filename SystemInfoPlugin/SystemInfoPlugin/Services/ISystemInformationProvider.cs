namespace SystemInfoPlugin.Services;

/// <summary>
/// Provides information about the system where the plugin is running.
/// </summary>
public interface ISystemInformationProvider
{
    /// <summary>
    /// Gets the machine name.
    /// </summary>
    string GetMachineName();
}
