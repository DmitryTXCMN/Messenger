using Microsoft.AspNetCore.Mvc;
using Planscam.Entities;

namespace Planscam.MobileApi.Models;

public class DeleteTrackViewModel
{
    public Track Track { get; set; } = null!;
    [HiddenInput(DisplayValue = false)] public string? ReturnUrl { get; set; }
}
