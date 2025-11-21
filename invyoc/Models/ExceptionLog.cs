namespace invyoc.Models;

public class ExceptionLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? ExceptionType { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}