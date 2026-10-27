using System.Text.Json.Serialization;
using SystemMetricsPlugin.Models;

namespace SystemMetricsPlugin;

[JsonSourceGenerationOptions(
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(TaskConfig))]
internal partial class PluginJsonSerializerContext : JsonSerializerContext;
