namespace PRN232.FUNewsManagementSystem.API.Models.Response.Tag;

/// <summary>
/// Response model for paginated tag list
/// </summary>
public class TagListResponse
{
    /// <summary>
    /// List of tags
    /// </summary>
    public required Pagination<TagResponse> Tags { get; set; }
    
    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = "Tags retrieved successfully";
}
