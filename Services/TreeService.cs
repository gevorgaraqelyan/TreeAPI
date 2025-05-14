using Microsoft.EntityFrameworkCore;
using TreeAPI.Exceptions;
using TreeAPI.Models;
using TreeAPI.Repositories;
using TreeAPI.Repositories.Interface;
using TreeAPI.Services.Interfaces;

namespace TreeAPI.Services;

public class TreeService : ITreeService
{
    private readonly ITreeRepository _treeRepository;

    public TreeService(ITreeRepository treeRepository)
    {
        _treeRepository = treeRepository;
    }

    public async Task<IEnumerable<Tree>> GetAllTreesAsync()
    {
        return await _treeRepository.GetAllAsync();
    }

    public async Task<Tree?> GetTreeByIdAsync(int id)
    {
        return await _treeRepository.GetTreeWithRootNodesAsync(id);
    }

    public async Task<Tree> CreateTreeAsync(Tree tree)
    {
        await _treeRepository.AddAsync(tree);
        await _treeRepository.SaveChangesAsync();
        return tree;
    }

    public async Task<Tree> UpdateTreeAsync(Tree tree)
    {
        var existingTree = await _treeRepository.GetByIdAsync(tree.Id);
        if (existingTree == null)
        {
            throw new SecureException($"Tree with ID {tree.Id} does not exist");
        }

        existingTree.Name = tree.Name;

        _treeRepository.Update(existingTree);
        await _treeRepository.SaveChangesAsync();
        return existingTree;
    }

    public async Task DeleteTreeAsync(int id)
    {
        var tree = await _treeRepository.GetByIdAsync(id);
        if (tree == null)
        {
            throw new SecureException($"Tree with ID {id} does not exist");
        }

        if (await _treeRepository.HasNodesAsync(id))
        {
            throw new SecureException("You have to delete all nodes in the tree first");
        }

        _treeRepository.Remove(tree);
        await _treeRepository.SaveChangesAsync();
    }
}