using HealthChecker.Dtos;

namespace HealthChecker.Services;

public class PrintService
{
    private const string LineaSeparadora = "----------------------------------------";

    public void PrintResults(
        List<HealthCheck> successfulEndpoints,
        List<HealthCheck> failedEndpoints
    )
    {
        PrintSummary(failedEndpoints);
        PrintSuccessfulEndpoints(successfulEndpoints);
        PrintFailedEndpoints(failedEndpoints);
    }

    private void PrintSummary(List<HealthCheck> failedEndpoints)
    {
        Console.WriteLine(LineaSeparadora);
        if (failedEndpoints.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Resumen: Todo OK. Las llamadas han respondido correctamente");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(LineaSeparadora);
            Console.WriteLine(
                "Resumen: KO, hay fallos. Las llamadas han producido errores. Revisa los detalles"
            );
            Console.WriteLine(LineaSeparadora);
        }
        Console.ResetColor();
        Console.WriteLine(LineaSeparadora);
    }

    private void PrintSuccessfulEndpoints(List<HealthCheck> successfulEndpoints)
    {
        if (successfulEndpoints.Count > 0)
        {
            Console.WriteLine(LineaSeparadora);
            Console.WriteLine("Detalle de llamadas correctas:");
            foreach (var endpoint in successfulEndpoints)
            {
                Console.WriteLine(
                    $"Alias: {endpoint.Alias} - Producto: {endpoint.Producto} - Status: Success - Endpoint: {endpoint.Url}"
                );
            }
            Console.WriteLine(LineaSeparadora);
        }
    }

    private void PrintFailedEndpoints(List<HealthCheck> failedEndpoints)
    {
        if (failedEndpoints.Count > 0)
        {
            Console.WriteLine(LineaSeparadora);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Detalle de llamadas con fallos:");
            foreach (var endpoint in failedEndpoints)
            {
                Console.WriteLine(
                    $"Alias: {endpoint.Alias} - Producto: {endpoint.Producto} - Status: Failed - Endpoint: {endpoint.Url} - Response: {endpoint.ResponseMessage}"
                );
            }
            Console.ResetColor();
            Console.WriteLine(LineaSeparadora);
        }
    }
}
