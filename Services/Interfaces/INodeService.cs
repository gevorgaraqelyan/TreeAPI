using TreeAPI.Models;

namespace TreeAPI.Services.Interfaces;

public interface INodeService
{
    Task<IEnumerable<Node>> GetAllRootNodesAsync(int treeId);
    Task<Node?> GetNodeByIdAsync(int id);
    Task<IEnumerable<Node>> GetChildrenAsync(int nodeId);
    Task<Node> CreateNodeAsync(Node node);
    Task<Node> UpdateNodeAsync(Node node);
    Task DeleteNodeAsync(int id);
    Task<Node> MoveNodeAsync(int nodeId, int? newParentId);
}