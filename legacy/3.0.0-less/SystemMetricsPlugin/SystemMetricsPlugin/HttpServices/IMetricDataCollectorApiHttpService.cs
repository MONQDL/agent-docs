namespace SystemMetricsPlugin.HttpServices;

/// <summary>
/// The interface of a service that operates with Metrics Data Collector API.
/// </summary>
public interface IMetricsDataCollectorApiHttpService
{
    /// <summary>
    /// Send metrics to Monq.
    /// </summary>
    /// <param name="baseUri">The base URI of the Monq system.</param>
    /// <param name="streamKey">The key of the target data stream.</param>
    /// <param name="userspaceId">The identifier of the target userspace.</param>
    /// <param name="metricsText">Metrics data in the Prometheus Text Based format.</param>
    public Task PushMetrics(string baseUri, string streamKey, long userspaceId, string metricsText);
}
