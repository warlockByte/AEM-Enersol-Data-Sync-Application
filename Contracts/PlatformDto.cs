using System.Text.Json.Serialization;

namespace AemenersolSync.Contracts;

public sealed class PlatformDto
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("uniqueName")] public string? UniqueName { get; init; }
    [JsonPropertyName("latitude")] public double? Latitude { get; init; }
    [JsonPropertyName("longitude")] public double? Longitude { get; init; }
    [JsonPropertyName("createdAt")] public DateTime? CreatedAt { get; init; }
    [JsonPropertyName("updatedAt")] public DateTime? UpdatedAt { get; init; }
    [JsonPropertyName("well")] public IReadOnlyList<WellDto>? Wells { get; init; }
}

public sealed class WellDto
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("platformId")] public int PlatformId { get; init; }
    [JsonPropertyName("uniqueName")] public string? UniqueName { get; init; }
    [JsonPropertyName("latitude")] public double? Latitude { get; init; }
    [JsonPropertyName("longitude")] public double? Longitude { get; init; }
    [JsonPropertyName("createdAt")] public DateTime? CreatedAt { get; init; }
    [JsonPropertyName("updatedAt")] public DateTime? UpdatedAt { get; init; }
}
