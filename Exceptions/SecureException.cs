using Microsoft.AspNetCore.Mvc;

namespace TreeAPI.Exceptions;

public class SecureException : Exception
{
    public string EventId { get; }

    public SecureException(string message) : base(message)
    {
        EventId = DateTime.UtcNow.Ticks.ToString();
    }

    public SecureException(string message, Exception innerException) : base(message, innerException)
    {
        EventId = DateTime.UtcNow.Ticks.ToString();
    }
}