using System.ComponentModel.DataAnnotations;

namespace Planscam.Entities;

public class Author
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public Picture? Picture { get; set; }

    [Required]
    public User? User { get; set; }
    
    public List<Track>? Tracks { get; set; }
}
