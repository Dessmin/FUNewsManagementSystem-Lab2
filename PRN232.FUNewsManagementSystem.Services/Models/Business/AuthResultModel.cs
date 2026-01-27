namespace PRN232.FUNewsManagementSystem.Services.Models.Business;

/// <summary>
/// Business model for authentication result (used in Service layer)
/// </summary>
public class AuthResultModel
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
