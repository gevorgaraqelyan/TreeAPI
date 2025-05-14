using TreeAPI.Models;

namespace TreeAPI.Services.Interfaces;

public interface ITreeService
{
    Task<IEnumerable<Tree>> GetAllTreesAsync();
    Task<Tree?> GetTreeByIdAsync(int id);
    Task<Tree> CreateTreeAsync(Tree tree);
    Task<Tree> UpdateTreeAsync(Tree tree);
    Task DeleteTreeAsync(int id);
}