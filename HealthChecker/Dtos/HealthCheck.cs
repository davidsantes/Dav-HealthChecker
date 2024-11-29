namespace HealthChecker.Dtos;

public class HealthCheck
{
    public string Alias { get; set; }
    public string Producto { get; set; }
    public string Url { get; set; }
    public int ExpectedStatusCode { get; set; }
    public string ResponseMessage { get; set; }
    public bool IsSuccess { get; set; }
}
