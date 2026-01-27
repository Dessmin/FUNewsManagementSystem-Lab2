namespace PRN232.FUNewsManagementSystem.API.Models.Response.Auth;

/// <summary>
/// Response model for user data (used in API layer)
/// </summary>
public class UserResponse
{
    public int AccountID { get; set; }
    public string AccountName { get; set; } = null!;
    public string? AccountEmail { get; set; }
    public int AccountRole { get; set; }
}
