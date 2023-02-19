using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Models;

namespace Planscam.Controllers;

public class TracksController : PsmControllerBase
{
    public TracksController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager)
        : base(dataContext, userManager, signInManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? id) =>
        id is null
            ? View()
            : await DataContext.Tracks
                .Select(t => new Track
                {
                    Id = t.Id,
                    Name = t.Name,
                    Picture = t.Picture,
                    Author = t.Author,
                    IsLiked = CurrentUserQueryable.Select(user => user.FavouriteTracks!.Tracks!.Contains(t)).First()
                })
                .FirstOrDefaultAsync(t => t.Id == id) is { } track
                ? View(new TrackIndexViewModel
                {
                    Track = track,
                    NotAddedPlaylists = await DataContext.Playlists
                        .Where(playlist =>
                            CurrentUserQueryable
                                .Include(user => user.OwnedPlaylists!.Playlists)
                                .First()
                                .OwnedPlaylists!.Playlists!.Contains(playlist)
                            && !playlist.Tracks!.Contains(track))
                        .ToListAsync()
                })
                : NotFound();

    [HttpGet]
    public async Task<IActionResult> Search(TrackSearchViewModel? model)
    {
        if (model is null || !ModelState.IsValid) return View(model);
        var tracks = DataContext.Tracks
            .Where(model.ByAuthors
                ? track => track.Author!.Name.Contains(model.Query)
                : track => track.Name.Contains(model.Query))
            .Skip(10 * (model.Page - 1))
            .Take(10);
        var tracksList = await tracks
            .Include(track => track.Picture)
            .Include(track => track.Author)
            .ToListAsync();
        if (SignInManager.IsSignedIn(User))
        {
            var likedTrackIds = await CurrentUserQueryable
                .Select(user => user.FavouriteTracks!.Tracks!.Select(track => track.Id).ToList())
                .FirstAsync();
            foreach (var track in tracksList)
                track.IsLiked = false;
            foreach (var trackId in likedTrackIds)
                if (tracksList.Find(t => t.Id == trackId) is { } track)
                    track.IsLiked = true;
        }

        model.Result = new Playlist
        {
            Name = $"Search result, query: '{model.Query}'",
            Picture = tracks.Select(track => track.Picture).FirstOrDefault(picture => picture != null),
            Tracks = tracksList
        };
        return View(model);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> AddTrackToFavourite(int id)
    {
        var track = await DataContext.Tracks.Where(track => track.Id == id).FirstOrDefaultAsync();
        if (track is null) return BadRequest();
        CurrentUser = await CurrentUserQueryable
            .Include(user =>
                user.FavouriteTracks!.Tracks!.Where(track1 => track1.Id == id))
            .FirstAsync();
        if (CurrentUser.FavouriteTracks!.Tracks!.Contains(track)) return BadRequest();
        CurrentUser.FavouriteTracks!.Tracks!.Add(track);
        await DataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> RemoveTrackFromFavourite(int id)
    {
        var favTracks = await DataContext.Users
            .Where(user => user.Id == CurrentUserId)
            .Include(user => user.FavouriteTracks!.Tracks!.Where(track => track.Id == id))
            .Select(user => user.FavouriteTracks!)
            .FirstAsync();
        if (!favTracks.Tracks!.Any()) return BadRequest();
        favTracks.Tracks!.Remove(favTracks.Tracks.First());
        await DataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTrackData(int id) =>
        await DataContext.Tracks
                .Select(track => new
                {
                    track.Id,
                    Author = track.Author!.Name,
                    track.Name,
                    IsLiked = CurrentUserQueryable
                        .Any(user => user.FavouriteTracks!.Tracks!.Contains(track)),
                    Picture = track.Picture!.Data,
                    track.Data!.Data
                })
                .FirstOrDefaultAsync(track => track.Id == id) switch
            {
                { } track => Json(track),
                _ => NotFound()
            };
}
