namespace HealthChecker.Dtos;

public class SmtpSettings
{
    public bool IsEnabled { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
