namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for NewsArticle (used in Service layer)
/// </summary>
public class NewsArticleModel
{
    public int NewsArticleID { get; set; }
    public string NewsTitle { get; set; } = null!;
    public string? Headline { get; set; }
    public DateTime CreatedDate { get; set; }
    public string NewsContent { get; set; } = null!;
    public string? NewsSource { get; set; }
    public int CategoryID { get; set; }
    public bool NewsStatus { get; set; }
    public int CreatedByID { get; set; }
    public int? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }
    
    // Navigation properties
    public string? CategoryName { get; set; }
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
    public int TagsCount { get; set; }
}
