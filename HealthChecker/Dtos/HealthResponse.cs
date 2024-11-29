using System.Text.Json.Serialization;

namespace HealthChecker.Dtos;

public class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("details")]
    public string Details { get; set; }
}