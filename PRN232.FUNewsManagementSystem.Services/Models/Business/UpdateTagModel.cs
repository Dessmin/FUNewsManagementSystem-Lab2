using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for updating an existing tag (used in Service layer)
/// </summary>
public class UpdateTagModel
{
    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
    public string TagName { get; set; } = null!;
    
    [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
    public string? Note { get; set; }
}
