using Planscam.Entities;

namespace Planscam.Models;

public class AddPlayedTrackPlaylistViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Picture Picture { get; set; } = null!;
    public bool IsTrackInPlaylist { get; set; }
}
