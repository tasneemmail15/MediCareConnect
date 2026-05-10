using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Project.Web.Models.User;
public class PasswordViewModel
{
    public int Id { get; set; }

    [Required]
    [Remote(action: "VerifyPassword", controller: "User")]
    public required string OldPassword { get; set; }
    
    [Required]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    public required string PasswordConfirm  { get; set; }

}
