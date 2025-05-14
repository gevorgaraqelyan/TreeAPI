using Microsoft.AspNetCore.Mvc;
using TreeAPI.Models;

namespace TreeAPI.Models;

public class Node
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public virtual Node? Parent { get; set; }
    public virtual ICollection<Node> Children { get; set; } = new List<Node>();
    public int TreeId { get; set; }
    public virtual Tree Tree { get; set; } = null!;
}