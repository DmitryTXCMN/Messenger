using System.ComponentModel.DataAnnotations;
using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class TrackSearchViewModel
{
    [Required] public string Query { get; set; } = null!;
    public bool ByAuthors { get; set; }
    [Range(1, 100000)] public int Page { get; set; } = 1;
    public Playlist? Result { get; set; }
}
