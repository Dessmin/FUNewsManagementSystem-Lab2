using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.API.Models.Response.NewsArticle;

/// <summary>
/// Response model for paginated news article list
/// </summary>
public class NewsArticleListResponse
{
    /// <summary>
    /// List of news articles
    /// </summary>
    public required Pagination<NewsArticleResponse> NewsArticles { get; set; }
    
    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = "News articles retrieved successfully";
}
