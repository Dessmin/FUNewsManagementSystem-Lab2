using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Tag;

/// <summary>
/// Request model for updating an existing tag
/// </summary>
public class UpdateTagRequest
{
    /// <summary>
    /// Tag name
    /// </summary>
    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
    public string TagName { get; set; } = null!;
    
    /// <summary>
    /// Tag note or description
    /// </summary>
    [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
    public string? Note { get; set; }
}
