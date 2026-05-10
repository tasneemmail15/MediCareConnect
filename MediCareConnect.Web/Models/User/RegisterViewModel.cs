using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace MediCareConnect.Models;

public class RegisterViewModel
{ 
    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [Remote(action: "VerifyEmailAvailable", controller: "User")]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    public required string PasswordConfirm  { get; set; }


}
