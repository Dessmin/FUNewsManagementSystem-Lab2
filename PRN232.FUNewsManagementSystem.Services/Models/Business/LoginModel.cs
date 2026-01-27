namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for login (used in Service layer)
/// </summary>
public class LoginModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
