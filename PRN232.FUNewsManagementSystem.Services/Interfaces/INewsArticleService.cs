using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for NewsArticle operations
/// </summary>
public interface INewsArticleService
{
    /// <summary>
    /// Get news article by ID
    /// </summary>
    Task<NewsArticleModel?> GetNewsArticleByIdAsync(int id);
    
    /// <summary>
    /// Get paginated list of news articles with search and filter
    /// </summary>
    Task<Pagination<NewsArticleModel>> GetNewsArticlesAsync(NewsArticleQueryModel query);
    
    /// <summary>
    /// Create new news article
    /// </summary>
    Task<NewsArticleModel> CreateNewsArticleAsync(CreateNewsArticleModel model, int createdByID);
    
    /// <summary>
    /// Update existing news article
    /// </summary>
    Task<NewsArticleModel> UpdateNewsArticleAsync(int id, UpdateNewsArticleModel model, int updatedByID);
    
    /// <summary>
    /// Delete news article
    /// </summary>
    Task<bool> DeleteNewsArticleAsync(int id);
    
    /// <summary>
    /// Get news articles by category ID
    /// </summary>
    Task<List<NewsArticleModel>> GetNewsArticlesByCategoryAsync(int categoryId);
    
    /// <summary>
    /// Get news articles by author (created by)
    /// </summary>
    Task<List<NewsArticleModel>> GetNewsArticlesByAuthorAsync(int authorId);
}
