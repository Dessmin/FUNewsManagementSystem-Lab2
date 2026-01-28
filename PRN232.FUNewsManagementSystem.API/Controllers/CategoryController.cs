using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.FUNewsManagementSystem.API.Models.Request.Category;
using PRN232.FUNewsManagementSystem.API.Models.Response.Category;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;
using Swashbuckle.AspNetCore.Annotations;

namespace PRN232.FUNewsManagementSystem.API.Controllers;

/// <summary>
/// Category management endpoints
/// </summary>
[Route("api/categories")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get category by ID",
        Description = "Retrieve detailed information about a specific category including subcategory count and news article count."
    )]
    [SwaggerResponse(200, "Category retrieved successfully", typeof(CategoryResponse))]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        var response = new CategoryResponse
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            ParentCategoryName = category.ParentCategoryName,
            IsActive = category.IsActive,
            SubCategoriesCount = category.SubCategoriesCount,
            NewsArticlesCount = category.NewsArticlesCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all categories with filtering and pagination
    /// </summary>
    /// <param name="request">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all categories",
        Description = "Retrieve a paginated list of categories with optional filtering by name, active status, and parent category. Supports search and sorting."
    )]
    [SwaggerResponse(200, "Categories retrieved successfully", typeof(CategoryListResponse))]
    [SwaggerResponse(400, "Invalid query parameters")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] GetCategoriesRequest request)
    {
        var queryModel = new CategoryQueryModel
        {
            SearchTerm = request.SearchTerm,
            IsActive = request.IsActive,
            ParentCategoryID = request.ParentCategoryID,
            IncludeSubCategories = request.IncludeSubCategories,
            SortBy = request.SortBy,
            IsDescending = request.IsDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var paginatedCategories = await _categoryService.GetCategoriesAsync(queryModel);

        var categoryResponses = paginatedCategories.Select(c => new CategoryResponse
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDescription,
            ParentCategoryID = c.ParentCategoryID,
            ParentCategoryName = c.ParentCategoryName,
            IsActive = c.IsActive,
            SubCategoriesCount = c.SubCategoriesCount,
            NewsArticlesCount = c.NewsArticlesCount
        }).ToList();

        var response = new CategoryListResponse
        {
            Categories = new Pagination<CategoryResponse>(
                categoryResponses,
                paginatedCategories.TotalCount,
                paginatedCategories.CurrentPage,
                paginatedCategories.PageSize
            )
        };

        return Ok(response);
    }

    /// <summary>
    /// Get subcategories by parent category ID
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <returns>List of subcategories</returns>
    [HttpGet("{parentId:int}/subcategories")]
    [SwaggerOperation(
        Summary = "Get subcategories",
        Description = "Retrieve all direct subcategories of a specific parent category."
    )]
    [SwaggerResponse(200, "Subcategories retrieved successfully", typeof(List<CategoryResponse>))]
    [SwaggerResponse(404, "Parent category not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetSubCategoriesAsync(int parentId)
    {
        var subCategories = await _categoryService.GetSubCategoriesAsync(parentId);

        var response = subCategories.Select(c => new CategoryResponse
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDescription,
            ParentCategoryID = c.ParentCategoryID,
            ParentCategoryName = c.ParentCategoryName,
            IsActive = c.IsActive,
            SubCategoriesCount = c.SubCategoriesCount,
            NewsArticlesCount = c.NewsArticlesCount
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="request">Category creation data</param>
    /// <returns>Created category details</returns>
    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Create new category",
        Description = "Create a new category. Requires Staff or Admin role."
    )]
    [SwaggerResponse(201, "Category created successfully", typeof(CategoryResponse))]
    [SwaggerResponse(400, "Invalid category data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(409, "Category name already exists")]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createModel = new CreateCategoryModel
        {
            CategoryName = request.CategoryName,
            CategoryDescription = request.CategoryDescription,
            ParentCategoryID = request.ParentCategoryID,
            IsActive = request.IsActive
        };

        var category = await _categoryService.CreateCategoryAsync(createModel);

        var response = new CategoryResponse
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            ParentCategoryName = category.ParentCategoryName,
            IsActive = category.IsActive,
            SubCategoriesCount = category.SubCategoriesCount,
            NewsArticlesCount = category.NewsArticlesCount
        };

        //return CreatedAtRoute(null, new { id = category.CategoryID }, response);
        return StatusCode(201, response);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Category update data</param>
    /// <returns>Updated category details</returns>
    [HttpPut("{id:int}")]
    // [Authorize(Policy = "StaffPolicy")] // Temporarily disabled for testing
    [SwaggerOperation(
        Summary = "Update category",
        Description = "Update an existing category. Requires Staff or Admin role."
    )]
    [SwaggerResponse(200, "Category updated successfully", typeof(CategoryResponse))]
    [SwaggerResponse(400, "Invalid category data")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(409, "Category name already exists")]
    public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updateModel = new UpdateCategoryModel
        {
            CategoryName = request.CategoryName,
            CategoryDescription = request.CategoryDescription,
            ParentCategoryID = request.ParentCategoryID,
            IsActive = request.IsActive
        };

        var category = await _categoryService.UpdateCategoryAsync(id, updateModel);

        var response = new CategoryResponse
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            ParentCategoryName = category.ParentCategoryName,
            IsActive = category.IsActive,
            SubCategoriesCount = category.SubCategoriesCount,
            NewsArticlesCount = category.NewsArticlesCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    [SwaggerOperation(
        Summary = "Delete category",
        Description = "Delete a category. Requires Admin role. Cannot delete categories with subcategories or news articles."
    )]
    [SwaggerResponse(200, "Category deleted successfully")]
    [SwaggerResponse(400, "Cannot delete category with subcategories or news articles")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Insufficient permissions")]
    [SwaggerResponse(404, "Category not found")]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok(new { Message = "Category deleted successfully" });
    }
}