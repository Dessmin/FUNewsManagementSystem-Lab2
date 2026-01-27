using System.ComponentModel.DataAnnotations;

namespace PRN232.FUNewsManagementSystem.API.Models.Request.Account;

/// <summary>
/// Request model for updating account (used in API layer)
/// </summary>
public class UpdateAccountRequest
{
    [Required(ErrorMessage = "Account name is required")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
    public string AccountName { get; set; } = null!;
    
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? AccountEmail { get; set; }
    
    [Range(0, 2, ErrorMessage = "Account role must be 0, 1, or 2")]
    public int AccountRole { get; set; }
}
