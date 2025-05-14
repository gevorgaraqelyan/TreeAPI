using Microsoft.AspNetCore.Mvc;
using TreeAPI.Models;

namespace TreeAPI.Models;

public class ExceptionLog
{
    public long Id { get; set; }
    public string EventId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string QueryParameters { get; set; } = string.Empty;
    public string BodyParameters { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
}