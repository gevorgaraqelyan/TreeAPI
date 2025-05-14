using Microsoft.AspNetCore.Mvc;
using TreeAPI.Models;

namespace TreeAPI.Models;

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public ErrorData Data { get; set; } = new ErrorData();
}