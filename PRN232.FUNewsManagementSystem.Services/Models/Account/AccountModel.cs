namespace PRN232.FUNewsManagementSystem.Services.Models.Account;

/// <summary>
/// Business model for system account (used in Service layer)
/// </summary>
public class AccountModel
{
    public int AccountID { get; set; }
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
}
