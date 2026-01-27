namespace PRN232.FUNewsManagementSystem.API.Models.Response.Category;

/// <summary>
/// Response model for category data
/// </summary>
public class CategoryResponse
{
    /// <summary>
    /// Category ID
    /// </summary>
    public int CategoryID { get; set; }
    
    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = null!;
    
    /// <summary>
    /// Category description
    /// </summary>
    public string? CategoryDescription { get; set; }
    
    /// <summary>
    /// Parent category ID
    /// </summary>
    public int? ParentCategoryID { get; set; }
    
    /// <summary>
    /// Parent category name
    /// </summary>
    public string? ParentCategoryName { get; set; }
    
    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Number of subcategories
    /// </summary>
    public int SubCategoriesCount { get; set; }
    
    /// <summary>
    /// Number of news articles in this category
    /// </summary>
    public int NewsArticlesCount { get; set; }
}