using Microsoft.EntityFrameworkCore;
using TreeAPI.Data;
using TreeAPI.Models;
using TreeAPI.Repositories.Interface;

namespace TreeAPI.Repositories;

public class TreeRepository : Repository<Tree>, ITreeRepository
{
    public TreeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tree?> GetTreeWithRootNodesAsync(int id)
    {
        return await _context.Trees
            .Include(t => t.Nodes.Where(n => n.ParentId == null))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> HasNodesAsync(int treeId)
    {
        return await _context.Nodes.AnyAsync(n => n.TreeId == treeId);
    }
}