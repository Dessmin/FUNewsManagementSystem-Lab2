using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Services;

/// <summary>
/// Service implementation for Tag operations
/// </summary>
public sealed class TagService : ITagService
{
    private readonly ILogger<TagService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork, ILogger<TagService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get tag by ID
    /// </summary>
    public async Task<TagModel?> GetTagByIdAsync(int id)
    {
        _logger.LogInformation($"Getting tag with ID: {id}");

        var tag = await _unitOfWork.TagRepository
            .GetAllAsQueryable()
            .Include(t => t.NewsTags)
            .FirstOrDefaultAsync(t => t.TagID == id);

        if (tag == null)
        {
            _logger.LogWarning($"Tag with ID {id} not found");
            throw ErrorHelper.NotFound($"Tag with ID {id} not found.");
        }

        return new TagModel
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = tag.NewsTags.Count
        };
    }

    /// <summary>
    /// Get paginated list of tags with search, filter, and sort
    /// </summary>
    public async Task<Pagination<TagModel>> GetTagsAsync(TagQueryModel query)
    {
        _logger.LogInformation($"Getting tags with query: SearchTerm={query.SearchTerm}");

        // Validate pagination parameters
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Min(100, Math.Max(1, query.PageSize));

        var baseQueryable = _unitOfWork.TagRepository
            .GetAllAsQueryable()
            .Include(t => t.NewsTags);

        IQueryable<Tag> queryable = baseQueryable;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(t => 
                t.TagName.ToLower().Contains(searchTerm) ||
                (t.Note != null && t.Note.ToLower().Contains(searchTerm)));
        }

        // Apply sorting
        queryable = ApplySorting(queryable, query.SortBy, query.IsDescending);

        // Get total count for pagination
        var totalCount = await queryable.CountAsync();

        // Apply pagination
        var tags = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Convert to business models
        var tagModels = tags.Select(t => new TagModel
        {
            TagID = t.TagID,
            TagName = t.TagName,
            Note = t.Note,
            NewsArticlesCount = t.NewsTags.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {tagModels.Count} tags out of {totalCount} total");

        return new Pagination<TagModel>(tagModels, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// Create new tag
    /// </summary>
    public async Task<TagModel> CreateTagAsync(CreateTagModel model)
    {
        _logger.LogInformation($"Creating tag: {model.TagName}");

        // Check if tag name already exists
        if (await _unitOfWork.TagRepository.FirstOrDefaultAsync(t => t.TagName == model.TagName) != null)
        {
            _logger.LogWarning($"Tag name {model.TagName} already exists");
            throw ErrorHelper.Conflict("Tag name already exists.");
        }

        var tag = new Tag
        {
            TagName = model.TagName,
            Note = model.Note
        };

        await _unitOfWork.TagRepository.CreateAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Tag created successfully with ID: {tag.TagID}");

        return new TagModel
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = 0
        };
    }

    /// <summary>
    /// Update existing tag
    /// </summary>
    public async Task<TagModel> UpdateTagAsync(int id, UpdateTagModel model)
    {
        _logger.LogInformation($"Updating tag with ID: {id}");

        var tag = await _unitOfWork.TagRepository
            .GetAllAsQueryable()
            .Include(t => t.NewsTags)
            .FirstOrDefaultAsync(t => t.TagID == id);

        if (tag == null)
        {
            _logger.LogWarning($"Tag with ID {id} not found");
            throw ErrorHelper.NotFound($"Tag with ID {id} not found.");
        }

        // Check if new tag name already exists (excluding current tag)
        if (await _unitOfWork.TagRepository.FirstOrDefaultAsync(t => t.TagName == model.TagName && t.TagID != id) != null)
        {
            _logger.LogWarning($"Tag name {model.TagName} already exists");
            throw ErrorHelper.Conflict("Tag name already exists.");
        }

        // Update tag properties
        tag.TagName = model.TagName;
        tag.Note = model.Note;

        await _unitOfWork.TagRepository.UpdateAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Tag updated successfully with ID: {tag.TagID}");

        return new TagModel
        {
            TagID = tag.TagID,
            TagName = tag.TagName,
            Note = tag.Note,
            NewsArticlesCount = tag.NewsTags.Count
        };
    }

    /// <summary>
    /// Delete tag
    /// </summary>
    public async Task<bool> DeleteTagAsync(int id)
    {
        _logger.LogInformation($"Deleting tag with ID: {id}");

        var tag = await _unitOfWork.TagRepository
            .GetAllAsQueryable()
            .Include(t => t.NewsTags)
            .FirstOrDefaultAsync(t => t.TagID == id);

        if (tag == null)
        {
            _logger.LogWarning($"Tag with ID {id} not found");
            throw ErrorHelper.NotFound($"Tag with ID {id} not found.");
        }

        // Check if tag has associated news articles
        if (tag.NewsTags.Any())
        {
            _logger.LogWarning($"Tag with ID {id} has associated news articles and cannot be deleted");
            throw ErrorHelper.BadRequest("Cannot delete tag that has associated news articles. Please remove tag from news articles first.");
        }

        await _unitOfWork.TagRepository.RemoveAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Tag deleted successfully with ID: {id}");

        return true;
    }

    private static IQueryable<Tag> ApplySorting(IQueryable<Tag> queryable, string? sortBy, bool isDescending)
    {
        return sortBy?.ToLower() switch
        {
            "tagname" => isDescending ? queryable.OrderByDescending(t => t.TagName) : queryable.OrderBy(t => t.TagName),
            "newscount" => isDescending 
                ? queryable.OrderByDescending(t => t.NewsTags.Count) 
                : queryable.OrderBy(t => t.NewsTags.Count),
            _ => isDescending ? queryable.OrderByDescending(t => t.TagID) : queryable.OrderBy(t => t.TagID)
        };
    }
}
