namespace PRN232.FUNewsManagementSystem.API.Models.Response.Auth;

/// <summary>
/// Response model for login (used in API layer)
/// </summary>
public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
}
