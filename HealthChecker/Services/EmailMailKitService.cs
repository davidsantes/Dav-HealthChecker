using System.Text;
using HealthChecker.Dtos;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HealthChecker.Services;

public class EmailMailKitService
{
    private const string LineaSeparadora = "----------------------------------------";
    private readonly IConfiguration _configuration;

    public EmailMailKitService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        List<HealthCheck> successfulEndpoints,
        List<HealthCheck> failedEndpoints
    )
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        var htmlBody = GenerateHtmlReport(successfulEndpoints, failedEndpoints);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.SenderEmail));
        message.To.Add(new MailboxAddress("", smtpSettings.ToEmail));
        message.Subject = smtpSettings.Subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = builder.ToMessageBody();

        Console.WriteLine(LineaSeparadora);
        Console.WriteLine("Enviando email...");
        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    smtpSettings.Server,
                    smtpSettings.Port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    "HealthCheckTesterKey",
                    "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"
                );

                await client.SendAsync(message);
            }
        }
        catch (SmtpCommandException ex)
        {
            Console.WriteLine($"Error enviando email: {ex.Message}");
            Console.WriteLine($"StatusCode: {ex.StatusCode}");
        }
        catch (SmtpProtocolException ex)
        {
            Console.WriteLine($"Error de protocolo mientras se envía el correo: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
        }
        Console.WriteLine(LineaSeparadora);
    }

    private string GenerateHtmlReport(
        List<HealthCheck> successfulEndpoints,
        List<HealthCheck> failedEndpoints
    )
    {
        var summary = failedEndpoints.Count == 0 ? "Todo OK." : "KO, hay fallos.";
        var html = new StringBuilder();

        html.Append("<html><body>");
        html.Append($"<h1>Resumen: {summary}</h1>");

        if (successfulEndpoints.Count > 0)
        {
            html.Append("<h2>Detalle de llamadas correctas:</h2><ul>");
            foreach (var endpoint in successfulEndpoints)
            {
                html.Append(
                    $"<li>Alias: {endpoint.Alias} - Producto: {endpoint.Producto} - Status: Success - Endpoint: {endpoint.Url}</li>"
                );
            }
            html.Append("</ul>");
        }

        if (failedEndpoints.Count > 0)
        {
            html.Append("<h2>Detalle de llamadas con fallos:</h2><ul>");
            foreach (var endpoint in failedEndpoints)
            {
                html.Append(
                    $"<li>Alias: {endpoint.Alias} - Producto: {endpoint.Producto} - Status: Failed - Endpoint: {endpoint.Url} - Response: {endpoint.ResponseMessage}</li>"
                );
            }
            html.Append("</ul>");
        }

        html.Append("</body></html>");
        return html.ToString();
    }
}
