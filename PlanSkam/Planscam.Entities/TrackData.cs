using System.ComponentModel.DataAnnotations;

namespace Planscam.Entities;

public class TrackData
{
    public int Id { get; set; }

    [Required]
    public Track? Track { get; set; }

    [Required]
    public byte[] Data { get; set; } = null!;
}
