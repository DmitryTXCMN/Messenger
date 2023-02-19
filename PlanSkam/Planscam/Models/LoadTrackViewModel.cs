using System.ComponentModel.DataAnnotations;
using Planscam.Entities;

namespace Planscam.Models;

public class LoadTrackViewModel
{
    [Required]
    public string? Name { get; set; }

    public IFormFile? Image { get; set; }

    [Required]
    public IFormFile? Track { get; set; }

    public List<Genre>? Genres { get; set; }

    [Required, ScaffoldColumn(false)]
    public int? GenreId { get; set; }
}
