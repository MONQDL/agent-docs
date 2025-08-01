using System.Collections;

namespace SystemInfoPlugin.HttpServices;

/// <summary>
/// The interface of a service that operates with Data Stream API.
/// </summary>
public interface IStreamDataCollectorApiHttpService
{
    /// <summary>
    /// Send events or logs to Monq.
    /// </summary>
    /// <param name="baseUri">The base URI of the Monq system.</param>
    /// <param name="streamKey">The key of the target data stream.</param>
    /// <param name="userspaceId">The identifier of the target userspace.</param>
    /// <param name="events">A collection of events or logs to send.</param>
    public Task PushEvents(string baseUri, string streamKey, long userspaceId, IList events);
}
