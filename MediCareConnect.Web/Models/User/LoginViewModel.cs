using System.ComponentModel.DataAnnotations;

namespace Project.Web.Models.User;  
public class LoginViewModel
{       
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

}
