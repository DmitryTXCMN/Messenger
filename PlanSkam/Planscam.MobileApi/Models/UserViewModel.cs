using Microsoft.AspNetCore.Mvc;
using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class UserViewModel
{
    [HiddenInput(DisplayValue = false)] public string Id { get; set; }
    public string Name { get; set; }
    [HiddenInput] public string Email { get; set; }
    public Picture? Picture { get; set; }
    
    public IFormFile? UploadImage { get; set; }
    
    public bool IsAuthor { get; set; } = false;
}
