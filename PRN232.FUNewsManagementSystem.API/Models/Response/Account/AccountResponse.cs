namespace PRN232.FUNewsManagementSystem.API.Models.Response.Account;

/// <summary>
/// Response model for account (used in API layer)
/// </summary>
public class AccountResponse
{
    public int AccountID { get; set; }
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
    public string AccountRoleName { get; set; } = null!;
}
