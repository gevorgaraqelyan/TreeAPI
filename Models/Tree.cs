using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace TreeAPI.Models;

public class Tree
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
}
