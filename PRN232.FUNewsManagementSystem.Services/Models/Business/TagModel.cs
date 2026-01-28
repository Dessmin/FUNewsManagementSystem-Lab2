namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for Tag (used in Service layer)
/// </summary>
public class TagModel
{
    public int TagID { get; set; }
    public string TagName { get; set; } = null!;
    public string? Note { get; set; }
    
    public int NewsArticlesCount { get; set; }
}
