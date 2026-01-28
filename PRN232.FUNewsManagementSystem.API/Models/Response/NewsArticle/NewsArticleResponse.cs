namespace PRN232.FUNewsManagementSystem.API.Models.Response.NewsArticle;

/// <summary>
/// Response model for news article data
/// </summary>
public class NewsArticleResponse
{
    /// <summary>
    /// News article ID
    /// </summary>
    public int NewsArticleID { get; set; }
    
    /// <summary>
    /// News article title
    /// </summary>
    public string NewsTitle { get; set; } = null!;
    
    /// <summary>
    /// News article headline/summary
    /// </summary>
    public string? Headline { get; set; }
    
    /// <summary>
    /// Date when the article was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Full news content
    /// </summary>
    public string NewsContent { get; set; } = null!;
    
    /// <summary>
    /// Source of the news
    /// </summary>
    public string? NewsSource { get; set; }
    
    /// <summary>
    /// Category ID
    /// </summary>
    public int CategoryID { get; set; }
    
    /// <summary>
    /// Category name
    /// </summary>
    public string? CategoryName { get; set; }
    
    /// <summary>
    /// Whether the news article is active/published
    /// </summary>
    public bool NewsStatus { get; set; }
    
    /// <summary>
    /// Author/Creator account ID
    /// </summary>
    public int CreatedByID { get; set; }
    
    /// <summary>
    /// Author/Creator account name
    /// </summary>
    public string? CreatedByName { get; set; }
    
    /// <summary>
    /// Editor/Updater account ID
    /// </summary>
    public int? UpdatedByID { get; set; }
    
    /// <summary>
    /// Editor/Updater account name
    /// </summary>
    public string? UpdatedByName { get; set; }
    
    /// <summary>
    /// Date when the article was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }
    
    /// <summary>
    /// Number of tags associated with this article
    /// </summary>
    public int TagsCount { get; set; }
}
