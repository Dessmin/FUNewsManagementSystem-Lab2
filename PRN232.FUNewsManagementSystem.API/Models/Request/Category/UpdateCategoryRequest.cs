using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Category;

/// <summary>
/// Request model for updating an existing category
/// </summary>
public class UpdateCategoryRequest
{
    /// <summary>
    /// Category name
    /// </summary>
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = null!;
    
    /// <summary>
    /// Category description
    /// </summary>
    [StringLength(500, ErrorMessage = "Category description cannot exceed 500 characters")]
    public string? CategoryDescription { get; set; }
    
    /// <summary>
    /// Parent category ID for sub-categories
    /// </summary>
    public int? ParentCategoryID { get; set; }
    
    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
}