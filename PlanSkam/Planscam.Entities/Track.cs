using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planscam.Entities;

public class Track
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required, ForeignKey("TrackDataId")]
    public TrackData? Data { get; set; }

    public Picture? Picture { get; set; }

    [Required]
    public Author? Author { get; set; }

    [Required]
    public List<Playlist>? Playlists { get; set; }

    [Required]
    public Genre? Genre { get; set; }

    [NotMapped]
    public bool? IsLiked { get; set; }
}
