using System.ComponentModel.DataAnnotations;

namespace Planscam.Entities;

public class Picture
{
    public int Id { get; set; }

    [Required]
    public byte[] Data { get; set; } = null!;
}
