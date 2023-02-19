using Planscam.Entities;

namespace Planscam.Models;

public class TrackIndexViewModel
{
    public Track Track { get; set; } = null!;
    public List<Playlist> NotAddedPlaylists { get; set; } = null!;
}
