using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planscam.Entities;

public class Playlist
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public Picture? Picture { get; set; }

    [Required]
    public List<Track>? Tracks { get; set; }

    [Required]
    public List<User>? Users { get; set; }

    public OwnedPlaylists? OwnedBy { get; set; }
    
    [Required]
    public bool IsAlbum { get; set; }
    
    [NotMapped]
    public bool? IsLiked { get; set; }
    
    [NotMapped]
    public bool? IsOwned { get; set; }
}
