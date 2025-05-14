using Microsoft.AspNetCore.Mvc;
using TreeAPI.DTOs;
using TreeAPI.Models;
using TreeAPI.Services;
using TreeAPI.Services.Interfaces;

namespace TreeAPI.Controllers;

[ApiController]
[Route("api/nodes")]
public class NodeController : ControllerBase
{
    private readonly INodeService _nodeService;

    public NodeController(INodeService nodeService)
    {
        _nodeService = nodeService;
    }

    [HttpGet("tree/{treeId}")]
    public async Task<ActionResult<SuccessResponse>> GetRootNodes(int treeId)
    {
        var nodes = await _nodeService.GetAllRootNodesAsync(treeId);
        var nodeDtos = nodes.Select(MapToNodeDto).ToList();

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<List<NodeDto>>
            {
                Message = "Root nodes retrieved successfully",
                Result = nodeDtos
            }
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SuccessResponse>> GetNode(int id)
    {
        var node = await _nodeService.GetNodeByIdAsync(id);
        if (node == null)
        {
            return NotFound();
        }

        var nodeDto = MapToNodeDto(node);

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<NodeDto>
            {
                Message = "Node retrieved successfully",
                Result = nodeDto
            }
        };

        return Ok(response);
    }

    [HttpGet("{id}/children")]
    public async Task<ActionResult<SuccessResponse>> GetChildrenNodes(int id)
    {
        var children = await _nodeService.GetChildrenAsync(id);
        var childrenDtos = children.Select(MapToNodeDto).ToList();

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<List<NodeDto>>
            {
                Message = "Children nodes retrieved successfully",
                Result = childrenDtos
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SuccessResponse>> CreateNode(CreateNodeDto createNodeDto)
    {
        var node = new Node
        {
            Name = createNodeDto.Name,
            ParentId = createNodeDto.ParentId,
            TreeId = createNodeDto.TreeId
        };

        var createdNode = await _nodeService.CreateNodeAsync(node);
        var nodeDto = MapToNodeDto(createdNode);

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<NodeDto>
            {
                Message = "Node created successfully",
                Result = nodeDto
            }
        };

        return CreatedAtAction(nameof(GetNode), new { id = createdNode.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SuccessResponse>> UpdateNode(int id, UpdateNodeDto updateNodeDto)
    {
        try
        {
            var existingNode = await _nodeService.GetNodeByIdAsync(id);
            if (existingNode == null)
            {
                return NotFound();
            }

            var node = new Node
            {
                Id = id,
                Name = updateNodeDto.Name,
                ParentId = updateNodeDto.ParentId,
                TreeId = existingNode.TreeId
            };

            var updatedNode = await _nodeService.UpdateNodeAsync(node);
            var nodeDto = MapToNodeDto(updatedNode);

            var responseId = DateTime.UtcNow.Ticks.ToString();
            var response = new SuccessResponse
            {
                Type = "Success",
                Id = responseId,
                Data = new SuccessData<NodeDto>
                {
                    Message = "Node updated successfully",
                    Result = nodeDto
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SuccessResponse>> DeleteNode(int id)
    {
        await _nodeService.DeleteNodeAsync(id);

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<object>
            {
                Message = "Node deleted successfully"
            }
        };

        return Ok(response);
    }

    [HttpPatch("{id}/move")]
    public async Task<ActionResult<SuccessResponse>> MoveNode(int id, MoveNodeDto moveNodeDto)
    {
        var movedNode = await _nodeService.MoveNodeAsync(id, moveNodeDto.NewParentId);
        var nodeDto = MapToNodeDto(movedNode);

        var responseId = DateTime.UtcNow.Ticks.ToString();
        var response = new SuccessResponse
        {
            Type = "Success",
            Id = responseId,
            Data = new SuccessData<NodeDto>
            {
                Message = "Node moved successfully",
                Result = nodeDto
            }
        };

        return Ok(response);
    }

    private static NodeDto MapToNodeDto(Node node)
    {
        return new NodeDto
        {
            Id = node.Id,
            Name = node.Name,
            ParentId = node.ParentId,
            TreeId = node.TreeId,
            Children = node.Children?.Select(MapToNodeDto).ToList() ?? new List<NodeDto>()
        };
    }
}