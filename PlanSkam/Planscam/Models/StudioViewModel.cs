using Planscam.Entities;

namespace Planscam.Models;

public class StudioViewModel
{
    public Author Author { get; set; } = null!;
    public Playlist OwnedTracks { get; set; } = null!;
}