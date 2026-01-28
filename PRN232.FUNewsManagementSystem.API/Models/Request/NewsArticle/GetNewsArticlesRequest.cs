using System.ComponentModel;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.NewsArticle;

/// <summary>
/// Request model for getting news articles with filtering and pagination
/// </summary>
public class GetNewsArticlesRequest
{
    /// <summary>
    /// Search term to filter by news title, headline, content, or source
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by publication status (active/published)
    /// </summary>
    public bool? NewsStatus { get; set; }
    
    /// <summary>
    /// Filter by category ID
    /// </summary>
    public int? CategoryID { get; set; }
    
    /// <summary>
    /// Filter by author/creator ID
    /// </summary>
    public int? CreatedByID { get; set; }
    
    /// <summary>
    /// Filter by created date from (inclusive)
    /// </summary>
    public DateTime? CreatedDateFrom { get; set; }
    
    /// <summary>
    /// Filter by created date to (inclusive)
    /// </summary>
    public DateTime? CreatedDateTo { get; set; }
    
    /// <summary>
    /// Sort field (NewsTitle, CreatedDate, ModifiedDate, NewsStatus, CategoryID)
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort in descending order
    /// </summary>
    [DefaultValue(false)]
    public bool IsDescending { get; set; }
    
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [DefaultValue(1)]
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Page size (max 100)
    /// </summary>
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
}
