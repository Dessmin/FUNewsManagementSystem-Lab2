namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for news article query parameters (used in Service layer)
/// </summary>
public class NewsArticleQueryModel
{
    public string? SearchTerm { get; set; }
    public bool? NewsStatus { get; set; }
    public int? CategoryID { get; set; }
    public int? CreatedByID { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
