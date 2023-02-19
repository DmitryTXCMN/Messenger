using System.ComponentModel.DataAnnotations;
using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class AuthorsSearchViewModel
{
    [Required] public string Query { get; set; } = null!;
    [Range(1, 100000)] public int Page { get; set; } = 1;
    public List<Author>? Result { get; set; }
}
