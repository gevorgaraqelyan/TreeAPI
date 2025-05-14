namespace TreeAPI.DTOs;



public class UpdateNodeDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}