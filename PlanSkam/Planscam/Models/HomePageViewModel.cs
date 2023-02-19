using Planscam.Entities;

namespace Planscam.Models;

public class HomePageViewModel
{
    public List<Playlist> BestPlaylists { get; set; } = null!;
    public List<Track> BestTracks { get; set; } = null!;
    public List<Subscription> Subscriptions { get; set; } = null!;
}
