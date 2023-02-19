using Planscam.Entities;

namespace Planscam.Models;

public class AlbumCreatingViewModel
{
    //todo
    public List<Track> Tracks { get; set; } = null!;
    public string? Name { get; set; }
    public IFormFile? Picture { get; set; }
}
