using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class TrackIndexViewModel
{
    public Track Track { get; set; } = null!;
    public List<Playlist> NotAddedPlaylists { get; set; } = null!;
}
