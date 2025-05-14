using TreeAPI.Models;

namespace TreeAPI.Repositories.Interface;

public interface INodeRepository : IRepository<Node>
{
    Task<IEnumerable<Node>> GetRootNodesByTreeIdAsync(int treeId);
    Task<Node?> GetNodeWithChildrenAsync(int id);
    Task<IEnumerable<Node>> GetChildrenAsync(int nodeId);
    Task<bool> IsDescendantAsync(int ancestorId, int potentialDescendantId);
    Task<bool> HasChildrenAsync(int nodeId);
}