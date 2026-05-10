using System.ComponentModel.DataAnnotations;

namespace Project.Web.Models.User;
public class ForgotPasswordViewModel
{
    [Required]
    public required string Email { get; set; }
    
}
