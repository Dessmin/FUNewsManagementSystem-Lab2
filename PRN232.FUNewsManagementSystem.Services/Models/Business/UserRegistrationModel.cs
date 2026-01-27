namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for user registration (used in Service layer)
/// </summary>
public class UserRegistrationModel
{
    public string AccountName { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int AccountRole { get; set; }
}
