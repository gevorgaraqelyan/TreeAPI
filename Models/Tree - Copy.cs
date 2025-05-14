namespace TreeAPI.Models;

public class SuccessResponse
{
    public string Type { get; set; } = "Success";
    public string Id { get; set; } = string.Empty;
    public object Data { get; set; } = new();
}

public class SuccessData<T>
{
    public string Message { get; set; } = string.Empty;
    public T? Result { get; set; } = default;
}