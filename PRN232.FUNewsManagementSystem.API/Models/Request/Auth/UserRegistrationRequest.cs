using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Auth;

/// <summary>
/// Request model for user registration (used in API layer)
/// </summary>
public class UserRegistrationRequest
{
    [Required]
    public string AccountName { get; set; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    
    public int AccountRole { get; set; } = 0;
}
