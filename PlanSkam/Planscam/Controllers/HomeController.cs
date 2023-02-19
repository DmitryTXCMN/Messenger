using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Models;

namespace Planscam.Controllers;

public class HomeController : PsmControllerBase
{
    public HomeController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager) :
        base(dataContext, userManager, signInManager)
    {
    }

    public async Task<IActionResult> Index()
    {
        var playlists = await (SignInManager.IsSignedIn(User)
                ? DataContext.Playlists
                    .Select(playlist => new Playlist
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Picture = playlist.Picture,
                        IsLiked = CurrentUserQueryable.Select(user => user.Playlists!.Contains(playlist)).First()
                    })
                : DataContext.Playlists
                    .Include(playlist => playlist.Picture))
            .OrderByDescending(playlist => DataContext.Users.Count(user => user.Playlists!.Contains(playlist)))
            .Take(15)
            .ToListAsync();
        var subs = await DataContext.Subscriptions.ToListAsync();
        var tracks = await DataContext.Tracks
            .OrderByDescending(track => DataContext.Users.Count(user => user.FavouriteTracks!.Tracks!.Contains(track)))
            .Select(track => new Track
            {
                Id = track.Id,
                Name = track.Name,
                Picture = track.Picture,
                Author = track.Author,
                IsLiked = SignInManager.IsSignedIn(User)
                    ? CurrentUserQueryable
                        .Select(user => user.FavouriteTracks!.Tracks!.Contains(track))
                        .First()
                    : null
            })
            .Take(15)
            .ToListAsync();
        return View(new HomePageViewModel
        {
            BestPlaylists = playlists,
            BestTracks = tracks,
            Subscriptions = subs
        });
    }

    public async Task<IActionResult> MainPage()
    {
        var playlists = await (SignInManager.IsSignedIn(User)
                ? DataContext.Playlists
                    .Select(playlist => new Playlist
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Picture = playlist.Picture,
                        IsLiked = CurrentUserQueryable.Select(user => user.Playlists!.Contains(playlist)).First()
                    })
                : DataContext.Playlists
                    .Include(playlist => playlist.Picture))
            .OrderByDescending(playlist => DataContext.Users.Count(user => user.Playlists!.Contains(playlist)))
            .Take(15)
            .ToListAsync();
        var subs = await DataContext.Subscriptions.ToListAsync();
        var tracks = await DataContext.Tracks
            .OrderByDescending(track => DataContext.Users.Count(user => user.FavouriteTracks!.Tracks!.Contains(track)))
            .Select(track => new Track
            {
                Id = track.Id,
                Name = track.Name,
                Picture = track.Picture,
                Author = track.Author,
                IsLiked = SignInManager.IsSignedIn(User)
                    ? CurrentUserQueryable
                        .Select(user => user.FavouriteTracks!.Tracks!.Contains(track))
                        .First()
                    : null
            })
            .Take(15)
            .ToListAsync();
        return View("MainPage", new HomePageViewModel
        {
            BestPlaylists = playlists,
            BestTracks = tracks,
            Subscriptions = subs
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});

    [HttpGet, Authorize(Roles = "Sub")]
    public async Task<IActionResult> Search(string query)
    {
        var playlists = await DataContext.Playlists
            .Include(playlist => playlist.Picture)
            .Where(playlist => playlist.Name.Contains(query)
                               && !DataContext.FavouriteTracks.Any(fav => fav.Id == playlist.Id))
            .ToListAsync();
        var tracks = new Playlist
        {
            Name = $"search result, query = {query}",
            Tracks = await DataContext.Tracks
                .Where(track => track.Name.Contains(query))
                .Select(track => new Track
                {
                    Id = track.Id,
                    Name = track.Name,
                    Picture = track.Picture,
                    Author = track.Author,
                    IsLiked = CurrentUserQueryable.Select(user => user.FavouriteTracks!.Tracks!.Contains(track)).First()
                })
                .ToListAsync()
        };
        var authors = await DataContext.Authors
            .Include(author => author.Picture)
            .Where(author => author.Name.Contains(query))
            .ToListAsync();
        return View("SearchResult", new SearchAllViewModel
        {
            Playlists = playlists,
            Tracks = tracks,
            Authors = authors
        });
    }
}
