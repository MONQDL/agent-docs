using BufferedFileReaderPlugin.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BufferedFileReaderPlugin;

[JsonSourceGenerationOptions(
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(TaskConfig))]
internal partial class PluginJsonSerializerContext : JsonSerializerContext;
