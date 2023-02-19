using System.ComponentModel.DataAnnotations;

namespace Planscam.MobileApi.Models;

public class CreatePlaylistViewModel
{
    [Required] public string Name { get; set; } = null!;
    public IFormFile? Picture { get; set; }
}
