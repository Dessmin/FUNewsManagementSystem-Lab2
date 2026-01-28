namespace PRN232.FUNewsManagementSystem.Services.Models.Account;

/// <summary>
/// Business model for creating account (used in Service layer)
/// </summary>
public class CreateAccountModel
{
    public string AccountName { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int AccountRole { get; set; }
}
