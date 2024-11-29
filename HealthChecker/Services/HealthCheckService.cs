using HealthChecker.Dtos;
using System.Text.Json;

namespace HealthChecker.Services;

public class HealthCheckService
{
    private readonly HttpClient _httpClient;

    public HealthCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(
        List<HealthCheck> successfulEndpoints,
        List<HealthCheck> failedEndpoints
    )> CheckHealthAsync(List<HealthCheck> endpoints)
    {
        var successfulEndpoints = new List<HealthCheck>();
        var failedEndpoints = new List<HealthCheck>();

        foreach (var endpoint in endpoints)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint.Url);
                endpoint.ResponseMessage = await response.Content.ReadAsStringAsync();

                endpoint.IsSuccess = (int)response.StatusCode == endpoint.ExpectedStatusCode;

                if (endpoint.IsSuccess)
                {
                    try
                    {
                        var healthResponse = JsonSerializer.Deserialize<HealthResponse>(endpoint.ResponseMessage);

                        if (healthResponse != null)
                        {
                            if (healthResponse.Status.Equals("Unhealthy", StringComparison.OrdinalIgnoreCase))
                            {
                                endpoint.IsSuccess = false;
                                endpoint.ResponseMessage = $"El endpoint respondió con un estado no saludable: {healthResponse.Status}" +
                                    (string.IsNullOrEmpty(healthResponse.Details) ? "" : $", detalles: {healthResponse.Details}");
                            }
                            else if (!healthResponse.Status.Equals("Healthy", StringComparison.OrdinalIgnoreCase))
                            {
                                endpoint.IsSuccess = false;
                                endpoint.ResponseMessage = $"Estado desconocido: {healthResponse.Status}";
                            }
                        }
                        else
                        {
                            endpoint.IsSuccess = false;
                            endpoint.ResponseMessage = "La respuesta no contiene información válida de estado.";
                        }
                    }
                    catch (JsonException)
                    {
                        endpoint.IsSuccess = false;
                        endpoint.ResponseMessage = "La respuesta no es un JSON válido o no contiene información esperada.";
                    }
                }
                else
                {
                    endpoint.ResponseMessage =
                        $"Status code esperado: {endpoint.ExpectedStatusCode}. Status code recibido: {(int)response.StatusCode}";
                }
            }
            catch (HttpRequestException e)
            {
                endpoint.ResponseMessage = e.Message;
                endpoint.IsSuccess = false;
            }

            if (endpoint.IsSuccess)
            {
                successfulEndpoints.Add(endpoint);
            }
            else
            {
                failedEndpoints.Add(endpoint);
            }
        }

        return (successfulEndpoints, failedEndpoints);
    }
}