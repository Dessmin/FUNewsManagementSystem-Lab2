namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for Category (used in Service layer)
/// </summary>
public class CategoryModel
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryDescription { get; set; }
    public int? ParentCategoryID { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public string? ParentCategoryName { get; set; }
    public int SubCategoriesCount { get; set; }
    public int NewsArticlesCount { get; set; }
}