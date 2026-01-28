using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.FUNewsManagementSystem.API.Models.Request.NewsArticle;
using PRN232.FUNewsManagementSystem.API.Models.Response.NewsArticle;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers;

/// <summary>
/// News article management endpoints
/// </summary>
[Route("api/news-articles")]
[ApiController]
[Authorize]
public class NewsArticleController : ControllerBase
{
    private readonly INewsArticleService _newsArticleService;
    private readonly IClaimsService _claimsService;

    public NewsArticleController(INewsArticleService newsArticleService, IClaimsService claimsService)
    {
        _newsArticleService = newsArticleService;
        _claimsService = claimsService;
    }

    /// <summary>
    /// Get news article by ID
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <returns>News article details</returns>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get news article by ID",
        Description = "Retrieve detailed information about a specific news article including author, category, and tags count."
    )]
    [SwaggerResponse(200, "News article retrieved successfully", typeof(NewsArticleResponse))]
    [SwaggerResponse(404, "News article not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetNewsArticleByIdAsync(int id)
    {
        var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);

        var response = new NewsArticleResponse
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            CategoryName = newsArticle.CategoryName,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            CreatedByName = newsArticle.CreatedByName,
            UpdatedByID = newsArticle.UpdatedByID,
            UpdatedByName = newsArticle.UpdatedByName,
            ModifiedDate = newsArticle.ModifiedDate,
            TagsCount = newsArticle.TagsCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all news articles with filtering and pagination
    /// </summary>
    /// <param name="request">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of news articles</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all news articles",
        Description = "Retrieve a paginated list of news articles with optional filtering by title, status, category, author, and date range. Supports search and sorting."
    )]
    [SwaggerResponse(200, "News articles retrieved successfully", typeof(NewsArticleListResponse))]
    [SwaggerResponse(400, "Invalid query parameters")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetNewsArticlesAsync([FromQuery] GetNewsArticlesRequest request)
    {
        var queryModel = new NewsArticleQueryModel
        {
            SearchTerm = request.SearchTerm,
            NewsStatus = request.NewsStatus,
            CategoryID = request.CategoryID,
            CreatedByID = request.CreatedByID,
            CreatedDateFrom = request.CreatedDateFrom,
            CreatedDateTo = request.CreatedDateTo,
            SortBy = request.SortBy,
            IsDescending = request.IsDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var paginatedNewsArticles = await _newsArticleService.GetNewsArticlesAsync(queryModel);

        var newsArticleResponses = paginatedNewsArticles.Select(n => new NewsArticleResponse
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            CategoryName = n.CategoryName,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            CreatedByName = n.CreatedByName,
            UpdatedByID = n.UpdatedByID,
            UpdatedByName = n.UpdatedByName,
            ModifiedDate = n.ModifiedDate,
            TagsCount = n.TagsCount
        }).ToList();

        var response = new NewsArticleListResponse
        {
            NewsArticles = new Pagination<NewsArticleResponse>(
                newsArticleResponses,
                paginatedNewsArticles.TotalCount,
                paginatedNewsArticles.CurrentPage,
                paginatedNewsArticles.PageSize
            )
        };

        return Ok(response);
    }

    /// <summary>
    /// Get news articles by category ID
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of news articles in the category</returns>
    [HttpGet("category/{categoryId:int}")]
    [SwaggerOperation(
        Summary = "Get news articles by category",
        Description = "Retrieve all news articles belonging to a specific category."
    )]
    [SwaggerResponse(200, "News articles retrieved successfully", typeof(List<NewsArticleResponse>))]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetNewsArticlesByCategoryAsync(int categoryId)
    {
        var newsArticles = await _newsArticleService.GetNewsArticlesByCategoryAsync(categoryId);

        var response = newsArticles.Select(n => new NewsArticleResponse
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            CategoryName = n.CategoryName,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            CreatedByName = n.CreatedByName,
            UpdatedByID = n.UpdatedByID,
            UpdatedByName = n.UpdatedByName,
            ModifiedDate = n.ModifiedDate,
            TagsCount = n.TagsCount
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get news articles by author ID
    /// </summary>
    /// <param name="authorId">Author/Creator account ID</param>
    /// <returns>List of news articles created by the author</returns>
    [HttpGet("author/{authorId:int}")]
    [SwaggerOperation(
        Summary = "Get news articles by author",
        Description = "Retrieve all news articles created by a specific author."
    )]
    [SwaggerResponse(200, "News articles retrieved successfully", typeof(List<NewsArticleResponse>))]
    [SwaggerResponse(404, "Author not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetNewsArticlesByAuthorAsync(int authorId)
    {
        var newsArticles = await _newsArticleService.GetNewsArticlesByAuthorAsync(authorId);

        var response = newsArticles.Select(n => new NewsArticleResponse
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            NewsContent = n.NewsContent,
            NewsSource = n.NewsSource,
            CategoryID = n.CategoryID,
            CategoryName = n.CategoryName,
            NewsStatus = n.NewsStatus,
            CreatedByID = n.CreatedByID,
            CreatedByName = n.CreatedByName,
            UpdatedByID = n.UpdatedByID,
            UpdatedByName = n.UpdatedByName,
            ModifiedDate = n.ModifiedDate,
            TagsCount = n.TagsCount
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Create a new news article
    /// </summary>
    /// <param name="request">News article creation data</param>
    /// <returns>Created news article details</returns>
    [HttpPost]
    [Authorize(Policy = "StaffPolicy")]
    [SwaggerOperation(
        Summary = "Create new news article",
        Description = "Create a new news article. Requires Staff or Admin role."
    )]
    [SwaggerResponse(201, "News article created successfully", typeof(NewsArticleResponse))]
    [SwaggerResponse(400, "Invalid news article data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Category or account not found")]
    public async Task<IActionResult> CreateNewsArticleAsync([FromBody] CreateNewsArticleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createModel = new CreateNewsArticleModel
        {
            NewsTitle = request.NewsTitle,
            Headline = request.Headline,
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource,
            CategoryID = request.CategoryID,
            NewsStatus = request.NewsStatus
        };

        var currentUserId = _claimsService.GetCurrentUserId;
        var newsArticle = await _newsArticleService.CreateNewsArticleAsync(createModel, currentUserId);

        var response = new NewsArticleResponse
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            CategoryName = newsArticle.CategoryName,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            CreatedByName = newsArticle.CreatedByName,
            UpdatedByID = newsArticle.UpdatedByID,
            UpdatedByName = newsArticle.UpdatedByName,
            ModifiedDate = newsArticle.ModifiedDate,
            TagsCount = newsArticle.TagsCount
        };

        return CreatedAtAction(nameof(GetNewsArticleByIdAsync), new { id = newsArticle.NewsArticleID }, response);
    }

    /// <summary>
    /// Update an existing news article
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <param name="request">News article update data</param>
    /// <returns>Updated news article details</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "StaffPolicy")]
    [SwaggerOperation(
        Summary = "Update news article",
        Description = "Update an existing news article. Requires Staff or Admin role."
    )]
    [SwaggerResponse(200, "News article updated successfully", typeof(NewsArticleResponse))]
    [SwaggerResponse(400, "Invalid news article data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "News article, category, or account not found")]
    public async Task<IActionResult> UpdateNewsArticleAsync(int id, [FromBody] UpdateNewsArticleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updateModel = new UpdateNewsArticleModel
        {
            NewsTitle = request.NewsTitle,
            Headline = request.Headline,
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource,
            CategoryID = request.CategoryID,
            NewsStatus = request.NewsStatus
        };

        var currentUserId = _claimsService.GetCurrentUserId;
        var newsArticle = await _newsArticleService.UpdateNewsArticleAsync(id, updateModel, currentUserId);

        var response = new NewsArticleResponse
        {
            NewsArticleID = newsArticle.NewsArticleID,
            NewsTitle = newsArticle.NewsTitle,
            Headline = newsArticle.Headline,
            CreatedDate = newsArticle.CreatedDate,
            NewsContent = newsArticle.NewsContent,
            NewsSource = newsArticle.NewsSource,
            CategoryID = newsArticle.CategoryID,
            CategoryName = newsArticle.CategoryName,
            NewsStatus = newsArticle.NewsStatus,
            CreatedByID = newsArticle.CreatedByID,
            CreatedByName = newsArticle.CreatedByName,
            UpdatedByID = newsArticle.UpdatedByID,
            UpdatedByName = newsArticle.UpdatedByName,
            ModifiedDate = newsArticle.ModifiedDate,
            TagsCount = newsArticle.TagsCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a news article
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Delete news article",
        Description = "Delete a news article. Requires Admin role. Associated tags will be automatically removed."
    )]
    [SwaggerResponse(200, "News article deleted successfully")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "News article not found")]
    public async Task<IActionResult> DeleteNewsArticleAsync(int id)
    {
        await _newsArticleService.DeleteNewsArticleAsync(id);
        return Ok(new { Message = "News article deleted successfully" });
    }
}
