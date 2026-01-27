using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN232.FUNewsManagementSystem.Services.Interfaces;
using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Services;

/// <summary>
/// Service implementation for Category operations
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    public async Task<CategoryModel?> GetCategoryByIdAsync(int id)
    {
        _logger.LogInformation($"Getting category with ID: {id}");

        var category = await _unitOfWork.CategoryRepository
            .GetAllAsQueryable()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.NewsArticles)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            _logger.LogWarning($"Category with ID {id} not found");
            throw ErrorHelper.NotFound($"Category with ID {id} not found.");
        }

        return new CategoryModel
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            IsActive = category.IsActive,
            ParentCategoryName = category.ParentCategory?.CategoryName,
            SubCategoriesCount = category.SubCategories.Count,
            NewsArticlesCount = category.NewsArticles.Count
        };
    }

    /// <summary>
    /// Get paginated list of categories with search, filter, and sort
    /// </summary>
    public async Task<Pagination<CategoryModel>> GetCategoriesAsync(CategoryQueryModel query)
    {
        _logger.LogInformation($"Getting categories with query: SearchTerm={query.SearchTerm}, IsActive={query.IsActive}, ParentCategoryID={query.ParentCategoryID}");

        // Validate pagination parameters
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Min(100, Math.Max(1, query.PageSize));

        var baseQueryable = _unitOfWork.CategoryRepository
            .GetAllAsQueryable()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.NewsArticles);

        // Convert to IQueryable<Category> for filtering
        IQueryable<Category> queryable = baseQueryable;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(c => 
                c.CategoryName.ToLower().Contains(searchTerm) ||
                (c.CategoryDescription != null && c.CategoryDescription.ToLower().Contains(searchTerm)));
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(c => c.IsActive == query.IsActive.Value);
        }

        if (query.ParentCategoryID.HasValue)
        {
            queryable = queryable.Where(c => c.ParentCategoryID == query.ParentCategoryID.Value);
        }
        else if (!query.IncludeSubCategories)
        {
            // If not including subcategories and no specific parent filter, show only root categories
            queryable = queryable.Where(c => c.ParentCategoryID == null);
        }

        // Apply sorting
        queryable = ApplySorting(queryable, query.SortBy, query.IsDescending);

        // Get total count for pagination
        var totalCount = await queryable.CountAsync();

        // Apply pagination
        var categories = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Convert to business models
        var categoryModels = categories.Select(c => new CategoryModel
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDescription,
            ParentCategoryID = c.ParentCategoryID,
            IsActive = c.IsActive,
            ParentCategoryName = c.ParentCategory?.CategoryName,
            SubCategoriesCount = c.SubCategories.Count,
            NewsArticlesCount = c.NewsArticles.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {categoryModels.Count} categories out of {totalCount} total");

        return new Pagination<CategoryModel>(categoryModels, totalCount, query.Page, query.PageSize);
    }

    /// <summary>
    /// Create new category
    /// </summary>
    public async Task<CategoryModel> CreateCategoryAsync(CreateCategoryModel model)
    {
        _logger.LogInformation($"Creating category: {model.CategoryName}");

        // Check if category name already exists
        if (await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.CategoryName == model.CategoryName) != null)
        {
            _logger.LogWarning($"Category name {model.CategoryName} already exists");
            throw ErrorHelper.Conflict("Category name already exists.");
        }

        // Validate parent category if specified
        if (model.ParentCategoryID.HasValue)
        {
            var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(model.ParentCategoryID.Value);
            if (parentCategory == null)
            {
                _logger.LogWarning($"Parent category with ID {model.ParentCategoryID} not found");
                throw ErrorHelper.NotFound($"Parent category with ID {model.ParentCategoryID} not found.");
            }
        }

        var category = new Category
        {
            CategoryName = model.CategoryName,
            CategoryDescription = model.CategoryDescription,
            ParentCategoryID = model.ParentCategoryID,
            IsActive = model.IsActive
        };

        await _unitOfWork.CategoryRepository.CreateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Category created successfully with ID: {category.CategoryID}");

        return new CategoryModel
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            IsActive = category.IsActive,
            SubCategoriesCount = 0,
            NewsArticlesCount = 0
        };
    }

    /// <summary>
    /// Update existing category
    /// </summary>
    public async Task<CategoryModel> UpdateCategoryAsync(int id, UpdateCategoryModel model)
    {
        _logger.LogInformation($"Updating category with ID: {id}");

        var category = await _unitOfWork.CategoryRepository
            .GetAllAsQueryable()
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.NewsArticles)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            _logger.LogWarning($"Category with ID {id} not found");
            throw ErrorHelper.NotFound($"Category with ID {id} not found.");
        }

        // Check if new category name already exists (excluding current category)
        if (await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.CategoryName == model.CategoryName && c.CategoryID != id) != null)
        {
            _logger.LogWarning($"Category name {model.CategoryName} already exists");
            throw ErrorHelper.Conflict("Category name already exists.");
        }

        // Validate parent category if specified
        if (model.ParentCategoryID.HasValue)
        {
            // Prevent self-reference
            if (model.ParentCategoryID == id)
            {
                _logger.LogWarning($"Category cannot be its own parent");
                throw ErrorHelper.BadRequest("Category cannot be its own parent.");
            }

            // Prevent circular reference (category cannot be child of its own subcategory)
            if (await IsCircularReferenceAsync(id, model.ParentCategoryID.Value))
            {
                _logger.LogWarning($"Circular reference detected");
                throw ErrorHelper.BadRequest("Circular reference detected. Category cannot be moved under its own subcategory.");
            }

            var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(model.ParentCategoryID.Value);
            if (parentCategory == null)
            {
                _logger.LogWarning($"Parent category with ID {model.ParentCategoryID} not found");
                throw ErrorHelper.NotFound($"Parent category with ID {model.ParentCategoryID} not found.");
            }
        }

        // Update category properties
        category.CategoryName = model.CategoryName;
        category.CategoryDescription = model.CategoryDescription;
        category.ParentCategoryID = model.ParentCategoryID;
        category.IsActive = model.IsActive;

        await _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Category updated successfully with ID: {category.CategoryID}");

        return new CategoryModel
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryID = category.ParentCategoryID,
            IsActive = category.IsActive,
            ParentCategoryName = category.ParentCategory?.CategoryName,
            SubCategoriesCount = category.SubCategories.Count,
            NewsArticlesCount = category.NewsArticles.Count
        };
    }

    /// <summary>
    /// Delete category
    /// </summary>
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        _logger.LogInformation($"Deleting category with ID: {id}");

        var category = await _unitOfWork.CategoryRepository
            .GetAllAsQueryable()
            .Include(c => c.SubCategories)
            .Include(c => c.NewsArticles)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            _logger.LogWarning($"Category with ID {id} not found");
            throw ErrorHelper.NotFound($"Category with ID {id} not found.");
        }

        // Check if category has subcategories
        if (category.SubCategories.Any())
        {
            _logger.LogWarning($"Category with ID {id} has subcategories and cannot be deleted");
            throw ErrorHelper.BadRequest("Cannot delete category that has subcategories. Please delete or reassign subcategories first.");
        }

        // Check if category has news articles
        if (category.NewsArticles.Any())
        {
            _logger.LogWarning($"Category with ID {id} has news articles and cannot be deleted");
            throw ErrorHelper.BadRequest("Cannot delete category that has news articles. Please move or delete news articles first.");
        }

        await _unitOfWork.CategoryRepository.RemoveAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Category deleted successfully with ID: {id}");

        return true;
    }

    /// <summary>
    /// Get subcategories by parent category ID
    /// </summary>
    public async Task<List<CategoryModel>> GetSubCategoriesAsync(int parentCategoryId)
    {
        _logger.LogInformation($"Getting subcategories for parent category ID: {parentCategoryId}");

        var subCategories = await _unitOfWork.CategoryRepository
            .GetAllAsQueryable()
            .Include(c => c.SubCategories)
            .Include(c => c.NewsArticles)
            .Where(c => c.ParentCategoryID == parentCategoryId)
            .ToListAsync();

        var categoryModels = subCategories.Select(c => new CategoryModel
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            CategoryDescription = c.CategoryDescription,
            ParentCategoryID = c.ParentCategoryID,
            IsActive = c.IsActive,
            SubCategoriesCount = c.SubCategories.Count,
            NewsArticlesCount = c.NewsArticles.Count
        }).ToList();

        _logger.LogInformation($"Retrieved {categoryModels.Count} subcategories");

        return categoryModels;
    }

    private static IQueryable<Category> ApplySorting(IQueryable<Category> queryable, string? sortBy, bool isDescending)
    {
        return sortBy?.ToLower() switch
        {
            "categoryname" => isDescending ? queryable.OrderByDescending(c => c.CategoryName) : queryable.OrderBy(c => c.CategoryName),
            "isactive" => isDescending ? queryable.OrderByDescending(c => c.IsActive) : queryable.OrderBy(c => c.IsActive),
            "parentcategoryid" => isDescending ? queryable.OrderByDescending(c => c.ParentCategoryID) : queryable.OrderBy(c => c.ParentCategoryID),
            _ => isDescending ? queryable.OrderByDescending(c => c.CategoryID) : queryable.OrderBy(c => c.CategoryID)
        };
    }

    private async Task<bool> IsCircularReferenceAsync(int categoryId, int newParentId)
    {
        var currentId = (int?)newParentId;
        while (currentId.HasValue)
        {
            if (currentId == categoryId)
            {
                return true; // Circular reference found
            }

            var parent = await _unitOfWork.CategoryRepository.GetByIdAsync(currentId.Value);
            currentId = parent?.ParentCategoryID;
        }

        return false; // No circular reference
    }
}