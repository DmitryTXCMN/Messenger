using Planscam.Entities;

namespace Planscam.Models;

public class TrackViewModel
{
    public Picture? Picture { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public bool? IsLiked { get; set; }
    public int? PlaylistId { get; set; }
}
