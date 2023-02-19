using Planscam.Entities;

namespace Planscam.Models;

public class SearchAllViewModel
{
    public List<Playlist> Playlists { get; set; } = null!;
    public Playlist Tracks { get; set; } = null!;
    public List<Author> Authors { get; set; } = null!;
}
