using System.ComponentModel;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Category;

/// <summary>
/// Request model for getting categories with filtering and pagination
/// </summary>
public class GetCategoriesRequest
{
    /// <summary>
    /// Search term to filter by category name or description
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Filter by parent category ID (null for root categories)
    /// </summary>
    public int? ParentCategoryID { get; set; }
    
    /// <summary>
    /// Include subcategories in the response
    /// </summary>
    [DefaultValue(true)]
    public bool IncludeSubCategories { get; set; } = true;
    
    /// <summary>
    /// Sort field (CategoryName, CategoryID, IsActive)
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort in descending order
    /// </summary>
    [DefaultValue(false)]
    public bool IsDescending { get; set; }
    
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [DefaultValue(1)]
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Page size (max 100)
    /// </summary>
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
}