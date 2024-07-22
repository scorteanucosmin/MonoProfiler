using System.Text.Json.Serialization;
using MonoProfiler.Models;

namespace Profiler.Models;

public class LocalStorageModel
{
    [JsonPropertyName("Root path")]
    public string? RootPath { get; set; }

    [JsonPropertyName("Registered assemblies")]
    public HashSet<string> RegisteredAssemblies { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName("Samples")]
    public List<ProfilingSample> Samples { get; set; } = new();
}