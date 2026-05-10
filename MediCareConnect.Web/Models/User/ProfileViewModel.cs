using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MediCareConnect.Models;
public class ProfileViewModel
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [Remote(action: "VerifyEmailAvailable", controller: "User", AdditionalFields = nameof(Id))]
    public required string Email { get; set; }

    public Role Role { get; set; }

}
