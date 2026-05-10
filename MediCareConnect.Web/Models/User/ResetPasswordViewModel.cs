using System.ComponentModel.DataAnnotations;

namespace MediCareConnect.Web.Models.User;
public class ResetPasswordViewModel
{
    [Required]
    public required string Email { get; set; }
    
    [Required]
    public required string Token { get; set; }

    [Required]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    public required string PasswordConfirm  { get; set; }
    
}
