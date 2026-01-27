namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for account query parameters (used in Service layer)
/// </summary>
public class AccountQueryModel
{
    public string? SearchTerm { get; set; }
    public int? AccountRole { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
