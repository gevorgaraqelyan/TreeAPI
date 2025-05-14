using Microsoft.AspNetCore.Mvc;
using TreeAPI.DTOs;
using TreeAPI.Exceptions;
using TreeAPI.Models;
using TreeAPI.Services.Interfaces;

namespace TreeAPI.Controllers;

[ApiController]
[Route("api/trees")]
public class TreeController : ControllerBase
{
    private readonly ITreeService _treeService;

    public TreeController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTrees()
    {
        var trees = await _treeService.GetAllTreesAsync();
        var treeList = trees.Select(t => new TreeDto
        {
            Id = t.Id,
            Name = t.Name
        }).ToList();

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<List<TreeDto>>
            {
                Message = "Trees retrieved successfully",
                Result = treeList
            }
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTree(int id)
    {
        var tree = await _treeService.GetTreeByIdAsync(id);
        if (tree == null)
        {
            throw new SecureException($"Tree with ID {id} does not exist");
        }

        var treeDto = new TreeDto
        {
            Id = tree.Id,
            Name = tree.Name
        };

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<TreeDto>
            {
                Message = "Tree retrieved successfully",
                Result = treeDto
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTree(CreateTreeDto createTreeDto)
    {
        var tree = new Tree
        {
            Name = createTreeDto.Name
        };

        var createdTree = await _treeService.CreateTreeAsync(tree);
        var treeDto = new TreeDto
        {
            Id = createdTree.Id,
            Name = createdTree.Name
        };

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<TreeDto>
            {
                Message = "Tree created successfully",
                Result = treeDto
            }
        };

        return CreatedAtAction(nameof(GetTree), new { id = createdTree.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTree(int id, UpdateTreeDto updateTreeDto)
    {
        var tree = new Tree
        {
            Id = id,
            Name = updateTreeDto.Name
        };

        var updatedTree = await _treeService.UpdateTreeAsync(tree);
        var treeDto = new TreeDto
        {
            Id = updatedTree.Id,
            Name = updatedTree.Name
        };

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<TreeDto>
            {
                Message = "Tree updated successfully",
                Result = treeDto
            }
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTree(int id)
    {
        await _treeService.DeleteTreeAsync(id);

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<object>
            {
                Message = "Tree deleted successfully"
            }
        };

        return Ok(response);
    }
}
