using System.Text.Json.Serialization;
using SystemInfoPlugin.Models;

namespace SystemInfoPlugin;

[JsonSourceGenerationOptions(
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(TaskConfig))]
internal partial class PluginJsonSerializerContext : JsonSerializerContext;
