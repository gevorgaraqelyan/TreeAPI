namespace TreeAPI.DTOs;


public class CreateNodeDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int TreeId { get; set; }
}