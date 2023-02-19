using System.ComponentModel.DataAnnotations;

namespace Planscam.MobileApi.Models;

public class RegisterViewModel
{
    [Required] public string UserName { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords are not same")]
    [DataType(DataType.Password)]
    [Display(Name = "Password one more time")]
    public string PasswordConfirm { get; set; } = null!;
}
