using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for Tag operations
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Get tag by ID
    /// </summary>
    Task<TagModel?> GetTagByIdAsync(int id);
    
    /// <summary>
    /// Get paginated list of tags with search and filter
    /// </summary>
    Task<Pagination<TagModel>> GetTagsAsync(TagQueryModel query);
    
    /// <summary>
    /// Create new tag
    /// </summary>
    Task<TagModel> CreateTagAsync(CreateTagModel model);
    
    /// <summary>
    /// Update existing tag
    /// </summary>
    Task<TagModel> UpdateTagAsync(int id, UpdateTagModel model);
    
    /// <summary>
    /// Delete tag
    /// </summary>
    Task<bool> DeleteTagAsync(int id);
}
