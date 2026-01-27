namespace PRN232.FUNewsManagementSystem.API.Models.Request.Account;

/// <summary>
/// Request model for querying accounts (used in API layer)
/// </summary>
public class GetAccountsRequest
{
    public string? searchTerm { get; set; }
    public int? accountRole { get; set; }
    public string? sortBy { get; set; }
    public bool isDescending { get; set; }
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 10;
}
