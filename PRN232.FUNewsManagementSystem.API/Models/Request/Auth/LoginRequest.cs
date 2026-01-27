using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Auth;

/// <summary>
/// Request model for login (used in API layer)
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}
