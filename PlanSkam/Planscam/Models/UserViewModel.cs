using Microsoft.AspNetCore.Mvc;
using Planscam.Entities;

namespace Planscam.Models;

public class UserViewModel
{
    [HiddenInput(DisplayValue = false)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Picture? Picture { get; set; }
    
    public IFormFile? UploadImage { get; set; }
    
    [HiddenInput(DisplayValue = false)]
    public bool IsAuthor { get; set; } = false;

    public DateTime? SubExpires;
}
