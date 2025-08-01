namespace SystemInfoPlugin.Models;

/// <summary>
/// Configurations of the task received by the agent.
/// </summary>
public class TaskConfig
{
    /// <summary>
    /// The base URI of the Monq system.
    /// </summary>
    public string BaseUri { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the target userspace.
    /// </summary>
    public long UserspaceId { get; set; }

    /// <summary>
    /// The key of the target data stream.
    /// </summary>
    public string StreamKey { get; set; } = string.Empty;
}
