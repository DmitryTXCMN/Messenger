using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Planscam.Models;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
         
    [HiddenInput(DisplayValue = false)]
    public string? ReturnUrl { get; set; }

    public List<AuthenticationScheme>? ExternalLogins { get; set; } = null!;
}
