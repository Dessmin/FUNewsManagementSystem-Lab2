namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for updating account (used in Service layer)
/// </summary>
public class UpdateAccountModel
{
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
}
