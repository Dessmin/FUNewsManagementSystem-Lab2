namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for category query parameters (used in Service layer)
/// </summary>
public class CategoryQueryModel
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public int? ParentCategoryID { get; set; }
    public bool IncludeSubCategories { get; set; } = true;
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}