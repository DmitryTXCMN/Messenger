using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Extensions;
using Planscam.FsServices;
using Planscam.Models;

namespace Planscam.Controllers;

public class PlaylistsController : PsmControllerBase
{
    private readonly PlaylistsRepo _playlistsRepo;

    public PlaylistsController(AppDbContext dataContext, UserManager<User> userManager,
        SignInManager<User> signInManager, PlaylistsRepo playlistsRepo) :
        base(dataContext, userManager, signInManager) =>
        _playlistsRepo = playlistsRepo;

    [HttpGet, Authorize]
    public async Task<IActionResult> FavoriteTracks() =>
        RedirectToAction("Index", new
        {
            Id = await _playlistsRepo.GetFavouriteTracksId(User)
        });

    [HttpGet]
    public async Task<IActionResult> Index(int id) =>
        await _playlistsRepo.GetPlaylistFull(id, User) switch
        {
            { } playlist => View(playlist),
            _ => NotFound()
        };

    [HttpGet]
    public async Task<IActionResult> All() =>
        View(await DataContext.Playlists
            .Where(playlist => DataContext.FavouriteTracks.All(tracks => tracks != playlist))
            .Include(playlist => playlist.Picture)
            .AsNoTracking()
            .Select(playlist => new Playlist
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Picture = playlist.Picture,
                IsAlbum = playlist.IsAlbum,
                IsLiked = SignInManager.IsSignedIn(User)
                    ? playlist.Users!.Any(user => user.Id == CurrentUserId)
                    : null,
                IsOwned = SignInManager.IsSignedIn(User)
                    ? CurrentUserQueryable
                        .Select(user => user.OwnedPlaylists!.Playlists!)
                        .Any(playlists => playlists.Any(playlist1 => playlist1 == playlist))
                    : null
            })
            .ToListAsync());

    [HttpGet, Authorize]
    public async Task<IActionResult> LayoutPlaylists() =>
        View(await _playlistsRepo.GetLikedPlaylists(User));

    [HttpPost, Authorize]
    public IActionResult LikePlaylist(int id) =>
        _playlistsRepo.LikePlaylist(User, id)
            ? Ok()
            : BadRequest();

    [HttpPost, Authorize]
    public IActionResult UnlikePlaylist(int id) =>
        _playlistsRepo.UnlikePlaylist(User, id)
            ? Ok()
            : BadRequest();

    [HttpGet, Authorize, Obsolete]
    public async Task<IActionResult> Liked()
    {
        CurrentUser = await CurrentUserQueryable
            .Include(user => user.Playlists!)
            .ThenInclude(playlist => playlist.Picture)
            .AsNoTracking()
            .FirstAsync();
        CurrentUser.Playlists!.ForEach(playlist => playlist.IsLiked = true);
        return View(CurrentUser);
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> Owned()
    {
        CurrentUser = await CurrentUserQueryable
            .Include(user => user.OwnedPlaylists!)
            .Include(user => user.FavouriteTracks!.Tracks!)
            .ThenInclude(track => track.Author)
            .Include(user => user.FavouriteTracks!.Tracks!)
            .ThenInclude(track => track.Picture)
            .AsNoTracking()
            .FirstAsync();
        CurrentUser.FavouriteTracks!.Tracks!.ForEach(track => track.IsLiked = true);
        CurrentUser.OwnedPlaylists!.Playlists = await DataContext.OwnedPlaylists
            .Where(playlists => playlists.Id == CurrentUser.OwnedPlaylists.Id)
            .Include(playlists => playlists.Playlists!)
            .ThenInclude(playlist => playlist.Picture)
            .Select(playlists => playlists.Playlists)
            .FirstAsync();
        return View(new OwnedPlaylistsViewModel
        {
            OwnedPlaylists = CurrentUser.OwnedPlaylists!.Playlists!,
            FavouriteTracks = CurrentUser.FavouriteTracks!
        });
    }

    [HttpGet, Authorize]
    public IActionResult Create() =>
        View();

    [HttpPost, Authorize]
    public IActionResult Create(CreatePlaylistViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        if (model.Picture is {Length: > 1600000})
        {
            ModelState.AddModelError("picture size", "picture size is too big");
            return View(model);
        }

        var playlist = _playlistsRepo.CreatePlaylist(User, model.Name, model.Picture.ToPicture());
        return View("CloseAndRedict", $"/Playlists/Index/{playlist.Id}");
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> Delete(int id) =>
        await DataContext.Playlists
            .AnyAsync(playlist =>
                playlist.Id == id && CurrentUserQueryable.Select(user => user.OwnedPlaylists!.Playlists!)
                    .Any(playlists => playlists.Contains(playlist)))
            ? View(new DeletePlaylistViewModel
            {
                Id = id,
                Name = "name"
            })
            : BadRequest();

    [HttpPost, Authorize]
    public IActionResult DeleteSure(int id) =>
        _playlistsRepo.DeletePlaylist(User, id)
            ? View("CloseAndRedict", $"/Home/MainPage")
            : BadRequest();

    [HttpPost, Authorize]
    public IActionResult AddTrackToPlaylist(int playlistId, int trackId) =>
        _playlistsRepo.AddTrackToPlaylist(User, playlistId, trackId)
            ? Ok()
            : BadRequest();

    //todo api
    [HttpPost, Authorize]
    public IActionResult RemoveTrackFromPlaylist(int playlistId, int trackId) =>
        _playlistsRepo.RemoveTrackFromPlaylist(User, playlistId, trackId)
            ? Ok()
            : BadRequest();

    [HttpGet, Authorize]
    public async Task<IActionResult> AddPlayedTrack(int trackId) =>
        View(await CurrentUserQueryable
            .Select(user => user.OwnedPlaylists!.Playlists!
                .Select(playlist =>
                    new AddPlayedTrackPlaylistViewModel
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Picture = playlist.Picture!,
                        IsTrackInPlaylist = playlist.Tracks!.Contains(new Track {Id = trackId})
                    })
                .ToList())
            .FirstAsync());

    [HttpGet]
    public IActionResult GetData(int id) =>
        _playlistsRepo.GetData(id) switch
        {
            { } playlist => Json(playlist),
            _ => NotFound()
        };

    [HttpGet]
    public async Task<IActionResult> IsTrackInPlaylist(int trackId, int playlistId) =>
        await DataContext.Playlists
                .Include(p => p.Tracks)
                .Where(p => p.Id == playlistId)
                .Select(p => p.Tracks!.Select(t => t.Id))
                .AsNoTracking()
                .FirstAsync()
            is { } trackIds
            ? Json(trackIds.Contains(trackId))
            : BadRequest();

    [HttpGet]
    public async Task<IActionResult> GenerateViewFromTrackIds(int[] ids)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var tracks = await DataContext.Tracks
            .Where(track => ids.Contains(track.Id))
            .Select(track => new Track
            {
                Id = track.Id,
                Name = track.Name,
                Picture = track.Picture,
                Author = track.Author,
                IsLiked = CurrentUserQueryable.Select(user => user.FavouriteTracks!.Tracks!.Contains(track)).First()
            })
            .ToListAsync();
        var playlist = new Playlist
        {
            Name = "Current playlist",
            Picture = tracks.Select(track => track.Picture).FirstOrDefault(picture => picture is { }),
            Tracks = tracks
        };
        return View(playlist);
    }
}
