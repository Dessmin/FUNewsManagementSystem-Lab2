namespace PRN232.FUNewsManagementSystem.API.Models.Response.Tag;

/// <summary>
/// Response model for tag data
/// </summary>
public class TagResponse
{
    /// <summary>
    /// Tag ID
    /// </summary>
    public int TagID { get; set; }
    
    /// <summary>
    /// Tag name
    /// </summary>
    public string TagName { get; set; } = null!;
    
    /// <summary>
    /// Tag note or description
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Number of news articles using this tag
    /// </summary>
    public int NewsArticlesCount { get; set; }
}
