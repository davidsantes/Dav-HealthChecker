using System.Text;
using HealthChecker.Dtos;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace HealthChecker.Services;

public class EmailSendGridService
{
    private const string LineaSeparadora = "----------------------------------------";
    private readonly IConfiguration _configuration;

    public EmailSendGridService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        List<HealthCheck> successfulEndpoints,
        List<HealthCheck> failedEndpoints
    )
    {
        var apiKey = "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";
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("test@example.com", "Example User");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress("davidsantes@hotmail.com", "Example User");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = GenerateHtmlReport(successfulEndpoints, failedEndpoints);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
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
