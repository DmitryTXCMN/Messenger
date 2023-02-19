using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class AuthorPageViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Picture Picture { get; set; } = null!;
    public List<Playlist> Albums { get; set; } = null!;
    public Playlist RecentReleases { get; set; } = null!;
}
