using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SystemMetricsPlugin.HttpServices.Implementation;

/// <summary>
/// The service that operates with Metrics Data Collector API.
/// </summary>
public class MetricsDataCollectorApiHttpService : IMetricsDataCollectorApiHttpService
{
    public const string StreamKeyHeader = "x-smon-stream-key";
    public const string UserspaceHeader = "X-Smon-Userspace-Id";
    const string MetricDataApiUri = "/api/public/mcs/v1/metrics-collector/prometheus/plain";

    static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(300);
    readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// The service's constructor.
    /// </summary>
    /// <param name="httpClientFactory">A factory that creates HttpClients</param>
    public MetricsDataCollectorApiHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task PushMetrics(string baseUri, string streamKey, long userspaceId, string metricsText)
    {
        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUri);
        client.DefaultRequestHeaders.TryAddWithoutValidation(UserspaceHeader, userspaceId.ToString());
        client.DefaultRequestHeaders.TryAddWithoutValidation(StreamKeyHeader, streamKey);

        var data = new StringContent(metricsText, Encoding.UTF8, Text.Plain);
        var response = await client.PostAsync(MetricDataApiUri, data, new CancellationTokenSource(_defaultTimeout).Token);
        response.EnsureSuccessStatusCode();
    }
}
