namespace PRN232.FUNewsManagementSystem.API.Models.Request.Tag;

/// <summary>
/// Request model for querying tags with pagination
/// </summary>
public class GetTagsRequest
{
    /// <summary>
    /// Search term to filter tags by name or note
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Field to sort by (TagName, NewsCount)
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort in descending order
    /// </summary>
    public bool IsDescending { get; set; }
    
    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page (default: 10, max: 100)
    /// </summary>
    public int PageSize { get; set; } = 10;
}
