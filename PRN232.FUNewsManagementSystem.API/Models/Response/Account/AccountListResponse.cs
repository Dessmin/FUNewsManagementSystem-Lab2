namespace PRN232.FUNewsManagementSystem.API.Models.Response.Account;

/// <summary>
/// Response model for paginated account list (used in API layer)
/// </summary>
public class AccountListResponse
{
    public List<AccountResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
