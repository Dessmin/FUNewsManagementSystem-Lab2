using PRN232.FUNewsManagementSystem.Services.Models.Business;

namespace PRN232.FUNewsManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for Category operations
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<CategoryModel?> GetCategoryByIdAsync(int id);
    
    /// <summary>
    /// Get paginated list of categories with search and filter
    /// </summary>
    Task<Pagination<CategoryModel>> GetCategoriesAsync(CategoryQueryModel query);
    
    /// <summary>
    /// Create new category
    /// </summary>
    Task<CategoryModel> CreateCategoryAsync(CreateCategoryModel model);
    
    /// <summary>
    /// Update existing category
    /// </summary>
    Task<CategoryModel> UpdateCategoryAsync(int id, UpdateCategoryModel model);
    
    /// <summary>
    /// Delete category
    /// </summary>
    Task<bool> DeleteCategoryAsync(int id);
    
    /// <summary>
    /// Get subcategories by parent category ID
    /// </summary>
    Task<List<CategoryModel>> GetSubCategoriesAsync(int parentCategoryId);
}