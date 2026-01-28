using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.NewsArticle;

/// <summary>
/// Request model for creating a new news article
/// </summary>
public class CreateNewsArticleRequest
{
    /// <summary>
    /// News article title
    /// </summary>
    [Required(ErrorMessage = "News title is required")]
    [StringLength(500, ErrorMessage = "News title cannot exceed 500 characters")]
    public string NewsTitle { get; set; } = null!;
    
    /// <summary>
    /// News article headline/summary
    /// </summary>
    [StringLength(1000, ErrorMessage = "Headline cannot exceed 1000 characters")]
    public string? Headline { get; set; }
    
    /// <summary>
    /// Full news content
    /// </summary>
    [Required(ErrorMessage = "News content is required")]
    public string NewsContent { get; set; } = null!;
    
    /// <summary>
    /// Source of the news
    /// </summary>
    [StringLength(200, ErrorMessage = "News source cannot exceed 200 characters")]
    public string? NewsSource { get; set; }
    
    /// <summary>
    /// Category ID for this news article
    /// </summary>
    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryID { get; set; }
    
    /// <summary>
    /// Whether the news article is active/published
    /// </summary>
    public bool NewsStatus { get; set; } = true;
}
