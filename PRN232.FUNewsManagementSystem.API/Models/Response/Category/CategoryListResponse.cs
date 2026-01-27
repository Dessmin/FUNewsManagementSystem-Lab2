namespace PRN232.FUNewsManagementSystem.API.Models.Response.Category;

/// <summary>
/// Response model for paginated category list
/// </summary>
public class CategoryListResponse
{
    /// <summary>
    /// List of categories
    /// </summary>
    public required Pagination<CategoryResponse> Categories { get; set; }
    
    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; set; } = "Categories retrieved successfully";
}