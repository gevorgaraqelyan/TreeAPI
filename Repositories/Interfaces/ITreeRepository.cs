using TreeAPI.Models;

namespace TreeAPI.Repositories.Interface;

public interface ITreeRepository : IRepository<Tree>
{
    Task<Tree?> GetTreeWithRootNodesAsync(int id);
    Task<bool> HasNodesAsync(int treeId);
}