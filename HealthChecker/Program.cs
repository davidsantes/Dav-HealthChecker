using HealthChecker.Dtos;
using HealthChecker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HealthChecker;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Resolve the services
        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var printService = host.Services.GetRequiredService<PrintService>();

        // Get health check endpoints from configuration
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var endpoints = configuration.GetSection("HealthCheckEndpoints").Get<List<HealthCheck>>();

        var (successfulEndpoints, failedEndpoints) = await healthCheckService.CheckHealthAsync(
            endpoints
        );

        printService.PrintResults(successfulEndpoints, failedEndpoints);

        var smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        if (smtpSettings.IsEnabled)
        {
            var emailService = host.Services.GetRequiredService<EmailSendGridService>();
            await emailService.SendEmailAsync(
                successfulEndpoints: successfulEndpoints,
                failedEndpoints: failedEndpoints
            );
        }

        Console.WriteLine("Presiona cualquier tecla para salir...");
        Console.ReadKey();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(
                (context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                }
            )
            .ConfigureServices(
                (context, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<HealthCheckService>();
                    services.AddSingleton<PrintService>();
                    services.AddSingleton<EmailMailKitService>();
                    services.AddSingleton<EmailSendGridService>();
                }
            );
}
