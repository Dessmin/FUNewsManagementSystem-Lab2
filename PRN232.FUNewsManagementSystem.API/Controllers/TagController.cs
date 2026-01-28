using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.FUNewsManagementSystem.API.Models.Request.Tag;
using PRN232.FUNewsManagementSystem.API.Models.Response.Tag;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers;

/// <summary>
/// Tag management endpoints
/// </summary>
[Route("api/tags")]
[ApiController]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Get tag by ID
    /// </summary>
    /// <param name="id">Tag ID</param>
    /// <returns>Tag details</returns>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get tag by ID",
        Description = "Retrieve detailed information about a specific tag including the count of news articles using this tag."
    )]
    [SwaggerResponse(200, "Tag retrieved successfully", typeof(TagResponse))]
    [SwaggerResponse(404, "Tag not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetTagByIdAsync(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);

        var response = new TagResponse
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = tag.NewsArticlesCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all tags with filtering and pagination
    /// </summary>
    /// <param name="request">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of tags</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all tags",
        Description = "Retrieve a paginated list of tags with optional filtering by name or note. Supports search and sorting."
    )]
    [SwaggerResponse(200, "Tags retrieved successfully", typeof(TagListResponse))]
    [SwaggerResponse(400, "Invalid query parameters")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetTagsAsync([FromQuery] GetTagsRequest request)
    {
        var queryModel = new TagQueryModel
        {
            SearchTerm = request.SearchTerm,
            SortBy = request.SortBy,
            IsDescending = request.IsDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var paginatedTags = await _tagService.GetTagsAsync(queryModel);

        var tagResponses = paginatedTags.Select(t => new TagResponse
        {
            TagID = t.TagID,
            TagName = t.TagName,
            Note = t.Note,
            NewsArticlesCount = t.NewsArticlesCount
        }).ToList();

        var response = new TagListResponse
        {
            Tags = new Pagination<TagResponse>(
                tagResponses,
                paginatedTags.TotalCount,
                paginatedTags.CurrentPage,
                paginatedTags.PageSize
            )
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new tag
    /// </summary>
    /// <param name="request">Tag creation data</param>
    /// <returns>Created tag details</returns>
    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Create new tag",
        Description = "Create a new tag. Requires Staff or Admin role."
    )]
    [SwaggerResponse(201, "Tag created successfully", typeof(TagResponse))]
    [SwaggerResponse(400, "Invalid tag data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(409, "Tag name already exists")]
    public async Task<IActionResult> CreateTagAsync([FromBody] CreateTagRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createModel = new CreateTagModel
        {
            TagName = request.TagName,
            Note = request.Note
        };

        var tag = await _tagService.CreateTagAsync(createModel);

        var response = new TagResponse
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = tag.NewsArticlesCount
        };

        return CreatedAtAction(nameof(GetTagByIdAsync), new { id = tag.TagID }, response);
    }

    /// <summary>
    /// Update an existing tag
    /// </summary>
    /// <param name="id">Tag ID</param>
    /// <param name="request">Tag update data</param>
    /// <returns>Updated tag details</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Update tag",
        Description = "Update an existing tag. Requires Staff or Admin role."
    )]
    [SwaggerResponse(200, "Tag updated successfully", typeof(TagResponse))]
    [SwaggerResponse(400, "Invalid tag data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(404, "Tag not found")]
    [SwaggerResponse(409, "Tag name already exists")]
    public async Task<IActionResult> UpdateTagAsync(int id, [FromBody] UpdateTagRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updateModel = new UpdateTagModel
        {
            TagName = request.TagName,
            Note = request.Note
        };

        var tag = await _tagService.UpdateTagAsync(id, updateModel);

        var response = new TagResponse
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = tag.NewsArticlesCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a tag
    /// </summary>
    /// <param name="id">Tag ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Delete tag",
        Description = "Delete a tag. Requires Admin role. Cannot delete tags that are associated with news articles."
    )]
    [SwaggerResponse(200, "Tag deleted successfully")]
    [SwaggerResponse(400, "Cannot delete tag with associated news articles")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(404, "Tag not found")]
    public async Task<IActionResult> DeleteTagAsync(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return Ok(new { Message = "Tag deleted successfully" });
    }
}
