using TreeAPI.Exceptions;
using TreeAPI.Models;
using TreeAPI.Repositories.Interface;
using TreeAPI.Services.Interfaces;


namespace TreeAPI.Services;

public class NodeService : INodeService
{
    private readonly INodeRepository _nodeRepository;
    private readonly ITreeRepository _treeRepository;
    private readonly ILogger<NodeService> _logger;

    public NodeService(INodeRepository nodeRepository, ITreeRepository treeRepository, ILogger<NodeService> logger)
    {
        _nodeRepository = nodeRepository;
        _treeRepository = treeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Node>> GetAllRootNodesAsync(int treeId)
    {
        return await _nodeRepository.GetRootNodesByTreeIdAsync(treeId);
    }

    public async Task<Node?> GetNodeByIdAsync(int id)
    {
        _logger.LogInformation($"Getting node with ID: {id}");
        var node = await _nodeRepository.GetNodeWithChildrenAsync(id);
        if (node == null)
        {
            _logger.LogWarning($"Node with ID {id} not found");
        }
        return node;
    }

    public async Task<IEnumerable<Node>> GetChildrenAsync(int nodeId)
    {
        return await _nodeRepository.GetChildrenAsync(nodeId);
    }

    public async Task<Node> CreateNodeAsync(Node node)
    {
        _logger.LogInformation($"Creating new node with name: {node.Name} in tree: {node.TreeId}");

        var tree = await _treeRepository.GetByIdAsync(node.TreeId);
        if (tree == null)
        {
            _logger.LogWarning($"Tree with ID {node.TreeId} not found");
            throw new SecureException($"Tree with ID {node.TreeId} does not exist");
        }

        if (node.ParentId.HasValue)
        {
            var parent = await _nodeRepository.GetByIdAsync(node.ParentId.Value);
            if (parent == null)
            {
                _logger.LogWarning($"Parent node with ID {node.ParentId.Value} not found");
                throw new SecureException($"Parent node with ID {node.ParentId.Value} does not exist");
            }

            if (parent.TreeId != node.TreeId)
            {
                _logger.LogWarning($"Parent node {node.ParentId.Value} is in tree {parent.TreeId}, not {node.TreeId}");
                throw new SecureException("Parent node must belong to the same tree as the child node");
            }
        }

        await _nodeRepository.AddAsync(node);
        await _nodeRepository.SaveChangesAsync();
        _logger.LogInformation($"Node created with ID: {node.Id}");
        return node;
    }

    public async Task<Node> UpdateNodeAsync(Node node)
    {
        _logger.LogInformation($"Updating node with ID: {node.Id}");

        var existingNode = await _nodeRepository.GetByIdAsync(node.Id);
        if (existingNode == null)
        {
            _logger.LogWarning($"Node with ID {node.Id} not found");
            throw new SecureException($"Node with ID {node.Id} does not exist");
        }

        if (existingNode.TreeId != node.TreeId)
        {
            _logger.LogWarning($"Attempt to change tree from {existingNode.TreeId} to {node.TreeId}");
            throw new SecureException("Cannot change the tree a node belongs to");
        }

        if (node.ParentId != existingNode.ParentId && node.ParentId.HasValue)
        {
            var parent = await _nodeRepository.GetByIdAsync(node.ParentId.Value);
            if (parent == null)
            {
                _logger.LogWarning($"Parent node with ID {node.ParentId.Value} not found");
                throw new SecureException($"Parent node with ID {node.ParentId.Value} does not exist");
            }

            if (parent.TreeId != node.TreeId)
            {
                _logger.LogWarning($"Parent node {node.ParentId.Value} is in tree {parent.TreeId}, not {node.TreeId}");
                throw new SecureException("Parent node must belong to the same tree as the child node");
            }

            if (await _nodeRepository.IsDescendantAsync(node.Id, node.ParentId.Value))
            {
                _logger.LogWarning($"Cannot move node {node.Id} to its descendant {node.ParentId.Value}");
                throw new SecureException("Cannot move a node to its own descendant");
            }
        }

        existingNode.Name = node.Name;
        existingNode.ParentId = node.ParentId;

        _nodeRepository.Update(existingNode);
        await _nodeRepository.SaveChangesAsync();
        _logger.LogInformation($"Node {node.Id} updated successfully");
        return existingNode;
    }

    public async Task DeleteNodeAsync(int id)
    {
        _logger.LogInformation($"Deleting node with ID: {id}");

        var node = await _nodeRepository.GetNodeWithChildrenAsync(id);
        if (node == null)
        {
            _logger.LogWarning($"Node with ID {id} not found");
            throw new SecureException($"Node with ID {id} does not exist");
        }

        if (await _nodeRepository.HasChildrenAsync(id))
        {
            _logger.LogWarning($"Cannot delete node {id} because it has children");
            throw new SecureException("You have to delete all children nodes first");
        }

        _nodeRepository.Remove(node);
        await _nodeRepository.SaveChangesAsync();
        _logger.LogInformation($"Node {id} deleted successfully");
    }

    public async Task<Node> MoveNodeAsync(int nodeId, int? newParentId)
    {
        _logger.LogInformation($"Moving node {nodeId} to new parent {newParentId}");

        var node = await _nodeRepository.GetByIdAsync(nodeId);
        if (node == null)
        {
            _logger.LogWarning($"Node with ID {nodeId} not found");
            throw new SecureException($"Node with ID {nodeId} does not exist");
        }

        if (newParentId.HasValue)
        {
            var parent = await _nodeRepository.GetByIdAsync(newParentId.Value);
            if (parent == null)
            {
                _logger.LogWarning($"Parent node with ID {newParentId.Value} not found");
                throw new SecureException($"Parent node with ID {newParentId.Value} does not exist");
            }

            if (parent.TreeId != node.TreeId)
            {
                _logger.LogWarning($"Parent node {newParentId.Value} is in tree {parent.TreeId}, not {node.TreeId}");
                throw new SecureException("Parent node must belong to the same tree as the child node");
            }

            if (await _nodeRepository.IsDescendantAsync(nodeId, newParentId.Value))
            {
                _logger.LogWarning($"Cannot move node {nodeId} to its descendant {newParentId.Value}");
                throw new SecureException("Cannot move a node to its own descendant");
            }
        }

        node.ParentId = newParentId;
        _nodeRepository.Update(node);
        await _nodeRepository.SaveChangesAsync();
        _logger.LogInformation($"Node {nodeId} moved successfully");
        return node;
    }
}