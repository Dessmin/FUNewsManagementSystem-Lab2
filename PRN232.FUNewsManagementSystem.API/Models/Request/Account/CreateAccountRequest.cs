using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Account;

/// <summary>
/// Request model for creating account (used in API layer)
/// </summary>
public class CreateAccountRequest
{
    [Required(ErrorMessage = "Account name is required")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
    public string AccountName { get; set; } = null!;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string AccountEmail { get; set; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = null!;
    
    [Range(0, 2, ErrorMessage = "Account role must be 0, 1, or 2")]
    public int AccountRole { get; set; }
}
