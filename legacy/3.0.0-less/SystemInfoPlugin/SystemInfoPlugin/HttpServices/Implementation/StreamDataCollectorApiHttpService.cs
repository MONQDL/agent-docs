using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace SystemInfoPlugin.HttpServices.Implementation;

/// <summary>
/// The service that operates with Data Stream API.
/// </summary>
public class StreamDataCollectorApiHttpService : IStreamDataCollectorApiHttpService
{
    const string UserspaceHeader = "X-Smon-Userspace-Id";
    const string StreamDataApiUri = "api/public/cl/v1/stream-data";
    static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
    readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// The service's constructor.
    /// </summary>
    /// <param name="httpClientFactory">A factory that creates HttpClients</param>
    public StreamDataCollectorApiHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task PushEvents(string baseUri, string streamKey, long userspaceId, IList events)
    {
        if (events.Count == 0)
            return;
        var json = JsonSerializer.Serialize(events, _serializerOptions);
        var data = new StringContent(json, Encoding.UTF8, Application.Json);

        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUri);
        client.DefaultRequestHeaders.TryAddWithoutValidation(UserspaceHeader, userspaceId.ToString());

        var requestUri = $"{StreamDataApiUri}?streamKey={streamKey}";
        var response = await client.PostAsync(requestUri, data, new CancellationTokenSource(_defaultTimeout).Token);
        response.EnsureSuccessStatusCode();
    }
}
