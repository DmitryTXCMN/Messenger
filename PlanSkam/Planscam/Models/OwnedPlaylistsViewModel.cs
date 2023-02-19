using Planscam.Entities;

namespace Planscam.Models;

public class OwnedPlaylistsViewModel
{
    public List<Playlist> OwnedPlaylists { get; set; }
    public Playlist FavouriteTracks { get; set; }
}
