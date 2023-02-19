using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;

namespace Planscam.MobileApi.Controllers;

public class TracksController : PsmControllerBase
{
    public TracksController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager)
        : base(dataContext, userManager, signInManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index(int id) =>
        await DataContext.Tracks
            .Include(t => t.Picture)
            .Include(t => t.Author)
            .FirstOrDefaultAsync(t => t.Id == id) is { } track
            ? Json(new
            {
                Track = new
                {
                    track.Id,
                    track.Name,
                    Author = new
                    {
                        track.Author!.Id,
                         track.Author.Name
                    },
                    track.Picture
                },
                NotAddedPlaylists = await DataContext.Playlists
                    .Where(playlist =>
                        CurrentUserQueryable
                            .Include(user => user.OwnedPlaylists!.Playlists)
                            .First()
                            .OwnedPlaylists!.Playlists!.Contains(playlist)
                        && !playlist.Tracks!.Contains(track))
                    .Select(playlist => new
                    {
                        playlist.Id,
                        playlist.Name
                    })
                    .ToListAsync()
            })
            : NotFound();

    [HttpGet]
    public async Task<IActionResult> Search(string query, int page, bool byAuthors)
    {
        page = page == 0 ? 1 : page;
        var tracks = DataContext.Tracks
            .Where(byAuthors
                ? track => track.Author!.Name.Contains(query)
                : track => track.Name.Contains(query))
            .Skip(10 * (page - 1))
            .Take(10);
        var tracksList = await tracks
            .Include(track => track.Picture)
            .Include(track => track.Author)
            .ToListAsync();
        if (IsSignedIn)
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

        var result = new
        {
            Name = $"Search result, query: '{query}'",
            Picture = tracks.Select(track => track.Picture).FirstOrDefault(picture => picture != null)?.Data,
            Tracks = tracksList.Select(track => new
            {
                track.Id,
                track.Name,
                track.Picture
            })
        };
        return Json(result);
    }

    [HttpPost, OpenIdDictAuthorize]
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

    [HttpPost, OpenIdDictAuthorize]
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
