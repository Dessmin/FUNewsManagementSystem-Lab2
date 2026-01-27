namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for user data (used in Service layer)
/// </summary>
public class UserModel
{
    public int AccountID { get; set; }
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
}
