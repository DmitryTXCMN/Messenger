using System.ComponentModel.DataAnnotations;

namespace Planscam.Models;

public class CreatePlaylistViewModel
{
    [Required] public string Name { get; set; } = null!;
    public IFormFile? Picture { get; set; }
}
