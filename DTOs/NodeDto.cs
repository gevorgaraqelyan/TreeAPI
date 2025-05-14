namespace TreeAPI.DTOs;

public class NodeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int TreeId { get; set; }
    public ICollection<NodeDto> Children { get; set; } = new List<NodeDto>();
}