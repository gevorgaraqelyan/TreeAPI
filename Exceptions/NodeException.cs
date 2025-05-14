using Microsoft.AspNetCore.Mvc;

namespace TreeAPI.Exceptions;

public class NodeException : SecureException
{
    public NodeException(string message) : base(message)
    {
    }

    public NodeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}