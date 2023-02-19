using System.ComponentModel.DataAnnotations;

namespace Planscam.Entities;

public class Genre
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public Picture? Picture { get; set; }
    public List<Track>? Tracks { get; set; }
}
