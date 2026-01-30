using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Services;

/// <summary>
/// Service implementation for NewsArticle operations
/// </summary>
public class NewsArticleService : INewsArticleService
{
    private readonly ILogger<NewsArticleService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NewsArticleService(IUnitOfWork unitOfWork, ILogger<NewsArticleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get news article by ID
    /// </summary>
    public async Task<NewsArticleModel?> GetNewsArticleByIdAsync(int id)
    {
        _logger.LogInformation($"Getting news article with ID: {id}");

        var newsArticle = await _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
            .FirstOrDefaultAsync(n => n.NewsArticleID == id);

        if (newsArticle == null)
        {
            _logger.LogWarning($"News article with ID {id} not found");
            throw ErrorHelper.NotFound($"News article with ID {id} not found.");
        }

        return new NewsArticleModel
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            UpdatedByID = newsArticle.UpdatedByID,
            ModifiedDate = newsArticle.ModifiedDate,
            CategoryName = newsArticle.Category?.CategoryName,
            CreatedByName = newsArticle.CreatedBy?.AccountName,
            UpdatedByName = newsArticle.UpdatedBy?.AccountName,
            TagsCount = newsArticle.NewsTags.Count
        };
    }

    /// <summary>
    /// Get paginated list of news articles with search, filter, and sort
    /// </summary>
    public async Task<Pagination<NewsArticleModel>> GetNewsArticlesAsync(NewsArticleQueryModel query)
    {
        _logger.LogInformation($"Getting news articles with query: SearchTerm={query.SearchTerm}, NewsStatus={query.NewsStatus}, CategoryID={query.CategoryID}");

        // Validate pagination parameters
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Min(100, Math.Max(1, query.PageSize));

        var baseQueryable = _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags);

        IQueryable<NewsArticle> queryable = baseQueryable;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(n => 
                n.NewsTitle.ToLower().Contains(searchTerm) ||
                (n.Headline != null && n.Headline.ToLower().Contains(searchTerm)) ||
                n.NewsContent.ToLower().Contains(searchTerm) ||
                (n.NewsSource != null && n.NewsSource.ToLower().Contains(searchTerm)));
        }

        if (query.NewsStatus.HasValue)
        {
            queryable = queryable.Where(n => n.NewsStatus == query.NewsStatus.Value);
        }

        if (query.CategoryID.HasValue)
        {
            queryable = queryable.Where(n => n.CategoryID == query.CategoryID.Value);
        }

        if (query.CreatedByID.HasValue)
        {
            queryable = queryable.Where(n => n.CreatedByID == query.CreatedByID.Value);
        }

        if (query.CreatedDateFrom.HasValue)
        {
            queryable = queryable.Where(n => n.CreatedDate >= query.CreatedDateFrom.Value);
        }

        if (query.CreatedDateTo.HasValue)
        {
            queryable = queryable.Where(n => n.CreatedDate <= query.CreatedDateTo.Value);
        }

        // Apply sorting
        queryable = ApplySorting(queryable, query.SortBy, query.IsDescending);

        // Get total count for pagination
        var totalCount = await queryable.CountAsync();

        // Apply pagination
        var newsArticles = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Convert to business models
        var newsArticleModels = newsArticles.Select(n => new NewsArticleModel
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            UpdatedByID = n.UpdatedByID,
            ModifiedDate = n.ModifiedDate,
            CategoryName = n.Category?.CategoryName,
            CreatedByName = n.CreatedBy?.AccountName,
            UpdatedByName = n.UpdatedBy?.AccountName,
            TagsCount = n.NewsTags.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {newsArticleModels.Count} news articles out of {totalCount} total");

        return new Pagination<NewsArticleModel>(newsArticleModels, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// Create new news article
    /// </summary>
    public async Task<NewsArticleModel> CreateNewsArticleAsync(CreateNewsArticleModel model, int createdByID)
    {
        _logger.LogInformation($"Creating news article: {model.NewsTitle}");

        // Validate category exists
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(model.CategoryID);
        if (category == null)
        {
            _logger.LogWarning($"Category with ID {model.CategoryID} not found");
            throw ErrorHelper.NotFound($"Category with ID {model.CategoryID} not found.");
        }

        // Validate category is active
        if (!category.IsActive)
        {
            _logger.LogWarning($"Category with ID {model.CategoryID} is not active");
            throw ErrorHelper.BadRequest("Cannot create news article under inactive category.");
        }

        // Validate created by account exists
        var createdBy = await _unitOfWork.AccountRepository.GetByIdAsync(createdByID);
        if (createdBy == null)
        {
            _logger.LogWarning($"Account with ID {createdByID} not found");
            throw ErrorHelper.NotFound($"Account with ID {createdByID} not found.");
        }

        var newsArticle = new NewsArticle
        {
            NewsTitle = model.NewsTitle,
            Headline = model.Headline,
            CreatedDate = DateTime.UtcNow,
            NewsContent = model.NewsContent,
            NewsSource = model.NewsSource,
            CategoryID = model.CategoryID,
            NewsStatus = model.NewsStatus,
            CreatedByID = createdByID,
            UpdatedByID = null,
            ModifiedDate = null
        };

        await _unitOfWork.NewsArticleRepository.CreateAsync(newsArticle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"News article created successfully with ID: {newsArticle.NewsArticleID}");

        return new NewsArticleModel
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            UpdatedByID = newsArticle.UpdatedByID,
            ModifiedDate = newsArticle.ModifiedDate,
            CategoryName = category.CategoryName,
            CreatedByName = createdBy.AccountName,
            TagsCount = 0
        };
    }

    /// <summary>
    /// Update existing news article
    /// </summary>
    public async Task<NewsArticleModel> UpdateNewsArticleAsync(int id, UpdateNewsArticleModel model, int updatedByID)
    {
        _logger.LogInformation($"Updating news article with ID: {id}");

        var newsArticle = await _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
            .FirstOrDefaultAsync(n => n.NewsArticleID == id);

        if (newsArticle == null)
        {
            _logger.LogWarning($"News article with ID {id} not found");
            throw ErrorHelper.NotFound($"News article with ID {id} not found.");
        }

        // Validate category exists
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(model.CategoryID);
        if (category == null)
        {
            _logger.LogWarning($"Category with ID {model.CategoryID} not found");
            throw ErrorHelper.NotFound($"Category with ID {model.CategoryID} not found.");
        }

        // Validate category is active
        if (!category.IsActive)
        {
            _logger.LogWarning($"Category with ID {model.CategoryID} is not active");
            throw ErrorHelper.BadRequest("Cannot update news article to inactive category.");
        }

        // Validate updated by account exists
        var updatedBy = await _unitOfWork.AccountRepository.GetByIdAsync(updatedByID);
        if (updatedBy == null)
        {
            _logger.LogWarning($"Account with ID {updatedByID} not found");
            throw ErrorHelper.NotFound($"Account with ID {updatedByID} not found.");
        }

        // Update news article properties
        newsArticle.NewsTitle = model.NewsTitle;
        newsArticle.Headline = model.Headline;
        newsArticle.NewsContent = model.NewsContent;
        newsArticle.NewsSource = model.NewsSource;
        newsArticle.Category = null;
        newsArticle.CategoryID = model.CategoryID;
        newsArticle.NewsStatus = model.NewsStatus;
        newsArticle.UpdatedByID = updatedByID;
        newsArticle.ModifiedDate = DateTime.UtcNow;

        await _unitOfWork.NewsArticleRepository.UpdateAsync(newsArticle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"News article updated successfully with ID: {newsArticle.NewsArticleID}");

        return new NewsArticleModel
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            UpdatedByID = newsArticle.UpdatedByID,
            ModifiedDate = newsArticle.ModifiedDate,
            CategoryName = category.CategoryName,
            CreatedByName = newsArticle.CreatedBy?.AccountName,
            UpdatedByName = updatedBy.AccountName,
            TagsCount = newsArticle.NewsTags.Count
        };
    }

    /// <summary>
    /// Delete news article
    /// </summary>
    public async Task<bool> DeleteNewsArticleAsync(int id)
    {
        _logger.LogInformation($"Deleting news article with ID: {id}");

        var newsArticle = await _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.NewsTags)
            .FirstOrDefaultAsync(n => n.NewsArticleID == id);

        if (newsArticle == null)
        {
            _logger.LogWarning($"News article with ID {id} not found");
            throw ErrorHelper.NotFound($"News article with ID {id} not found.");
        }

        // Delete associated news tags first
        if (newsArticle.NewsTags.Any())
        {
            foreach (var newsTag in newsArticle.NewsTags.ToList())
            {
                await _unitOfWork.NewsTagRepository.RemoveAsync(newsTag);
            }
        }

        await _unitOfWork.NewsArticleRepository.RemoveAsync(newsArticle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"News article deleted successfully with ID: {id}");

        return true;
    }

    /// <summary>
    /// Get news articles by category ID
    /// </summary>
    public async Task<List<NewsArticleModel>> GetNewsArticlesByCategoryAsync(int categoryId)
    {
        _logger.LogInformation($"Getting news articles for category ID: {categoryId}");

        var newsArticles = await _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
            .Where(n => n.CategoryID == categoryId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

        var newsArticleModels = newsArticles.Select(n => new NewsArticleModel
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            UpdatedByID = n.UpdatedByID,
            ModifiedDate = n.ModifiedDate,
            CategoryName = n.Category?.CategoryName,
            CreatedByName = n.CreatedBy?.AccountName,
            UpdatedByName = n.UpdatedBy?.AccountName,
            TagsCount = n.NewsTags.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {newsArticleModels.Count} news articles for category");

        return newsArticleModels;
    }

    /// <summary>
    /// Get news articles by author (created by)
    /// </summary>
    public async Task<List<NewsArticleModel>> GetNewsArticlesByAuthorAsync(int authorId)
    {
        _logger.LogInformation($"Getting news articles for author ID: {authorId}");

        var newsArticles = await _unitOfWork.NewsArticleRepository
            .GetAllAsQueryable()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
            .Where(n => n.CreatedByID == authorId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

        var newsArticleModels = newsArticles.Select(n => new NewsArticleModel
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            UpdatedByID = n.UpdatedByID,
            ModifiedDate = n.ModifiedDate,
            CategoryName = n.Category?.CategoryName,
            CreatedByName = n.CreatedBy?.AccountName,
            UpdatedByName = n.UpdatedBy?.AccountName,
            TagsCount = n.NewsTags.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {newsArticleModels.Count} news articles for author");

        return newsArticleModels;
    }

    private static IQueryable<NewsArticle> ApplySorting(IQueryable<NewsArticle> queryable, string? sortBy, bool isDescending)
    {
        return sortBy?.ToLower() switch
        {
            "newstitle" => isDescending ? queryable.OrderByDescending(n => n.NewsTitle) : queryable.OrderBy(n => n.NewsTitle),
            "createddate" => isDescending ? queryable.OrderByDescending(n => n.CreatedDate) : queryable.OrderBy(n => n.CreatedDate),
            "modifieddate" => isDescending ? queryable.OrderByDescending(n => n.ModifiedDate) : queryable.OrderBy(n => n.ModifiedDate),
            "newsstatus" => isDescending ? queryable.OrderByDescending(n => n.NewsStatus) : queryable.OrderBy(n => n.NewsStatus),
            "categoryid" => isDescending ? queryable.OrderByDescending(n => n.CategoryID) : queryable.OrderBy(n => n.CategoryID),
            _ => isDescending ? queryable.OrderByDescending(n => n.NewsArticleID) : queryable.OrderBy(n => n.NewsArticleID)
        };
    }
}
