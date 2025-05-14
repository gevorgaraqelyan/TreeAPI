using Microsoft.EntityFrameworkCore;
using TreeAPI.Data;
using TreeAPI.Models;
using TreeAPI.Repositories.Interface;

namespace TreeAPI.Repositories;

public class NodeRepository : Repository<Node>, INodeRepository
{
    public NodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Node>> GetRootNodesByTreeIdAsync(int treeId)
    {
        return await _context.Nodes
            .Where(n => n.TreeId == treeId && n.ParentId == null)
            .ToListAsync();
    }

    public async Task<Node?> GetNodeWithChildrenAsync(int id)
    {
        return await _context.Nodes
            .Include(n => n.Children)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Node>> GetChildrenAsync(int nodeId)
    {
        return await _context.Nodes
            .Where(n => n.ParentId == nodeId)
            .ToListAsync();
    }

    public async Task<bool> HasChildrenAsync(int nodeId)
    {
        return await _context.Nodes.AnyAsync(n => n.ParentId == nodeId);
    }

    public async Task<bool> IsDescendantAsync(int ancestorId, int potentialDescendantId)
    {
        var potentialDescendant = await _context.Nodes.FindAsync(potentialDescendantId);
        if (potentialDescendant == null)
        {
            return false;
        }

        if (potentialDescendant.ParentId == ancestorId)
        {
            return true;
        }

        if (potentialDescendant.ParentId.HasValue)
        {
            return await IsDescendantAsync(ancestorId, potentialDescendant.ParentId.Value);
        }

        return false;
    }
}