namespace SystemInfoPlugin.Services;

/// <summary>
/// Provides system information using the current runtime environment.
/// </summary>
public sealed class SystemInformationProvider : ISystemInformationProvider
{
    /// <inheritdoc/>
    public string GetMachineName() => Environment.MachineName;
}
