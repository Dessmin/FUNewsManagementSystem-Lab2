namespace PRN232.FUNewsManagementSystem.API.Models.Request.Account;

/// <summary>
/// Request model for querying accounts (used in API layer)
/// </summary>
public class GetAccountsRequest
{
    public string? SearchTerm { get; set; }
    public AccountRole? AccountRole { get; set; }
    public bool IsDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public enum AccountRole
{
    User = 0,
    Admin = 1,
    Staff = 2,
    Lecturer = 3
}
