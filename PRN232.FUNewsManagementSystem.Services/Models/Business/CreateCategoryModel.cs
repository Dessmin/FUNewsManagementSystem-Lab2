using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for creating a new category (used in Service layer)
/// </summary>
public class CreateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = null!;
    
    [StringLength(500, ErrorMessage = "Category description cannot exceed 500 characters")]
    public string? CategoryDescription { get; set; }
    
    public int? ParentCategoryID { get; set; }
    
    public bool IsActive { get; set; } = true;
}