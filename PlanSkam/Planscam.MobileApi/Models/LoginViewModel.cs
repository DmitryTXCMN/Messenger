using System.ComponentModel.DataAnnotations;

namespace Planscam.MobileApi.Models;

public class LoginViewModel
{
    [Required] 
    public string grant_type { get; set; } = null!;
    [Required]
    public string username { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; } = null!;
}
