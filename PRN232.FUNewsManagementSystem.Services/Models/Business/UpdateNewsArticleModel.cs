using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for updating an existing news article (used in Service layer)
/// </summary>
public class UpdateNewsArticleModel
{
    [Required(ErrorMessage = "News title is required")]
    [StringLength(500, ErrorMessage = "News title cannot exceed 500 characters")]
    public string NewsTitle { get; set; } = null!;
    
    [StringLength(1000, ErrorMessage = "Headline cannot exceed 1000 characters")]
    public string? Headline { get; set; }
    
    [Required(ErrorMessage = "News content is required")]
    public string NewsContent { get; set; } = null!;
    
    [StringLength(200, ErrorMessage = "News source cannot exceed 200 characters")]
    public string? NewsSource { get; set; }
    
    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryID { get; set; }
    
    public bool NewsStatus { get; set; }
}
