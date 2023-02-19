using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

public class PlaylistsController : PsmControllerBase
{
    private readonly PlaylistsRepo _playlistsRepo;

    public PlaylistsController(AppDbContext dataContext, UserManager<User> userManager,
        SignInManager<User> signInManager, PlaylistsRepo playlistsRepo) :
        base(dataContext, userManager, signInManager) =>
        _playlistsRepo = playlistsRepo;

    [HttpGet, OpenIdDictAuthorize]
    public async Task<IActionResult> FavoriteTracks() =>
        RedirectToAction("Index", new
        {
            Id = await _playlistsRepo.GetFavouriteTracksId(User)
        });

    [HttpGet]
    public async Task<IActionResult> Index(int id) =>
        await _playlistsRepo.GetPlaylistFull(id, User) switch
        {
            { } playlist => Json(playlist),
            _ => NotFound()
        };

    [HttpGet, OpenIdDictAuthorize, AllowAnonymous]
    public async Task<IActionResult> All() =>
        Json(await DataContext.Playlists
            .Where(playlist => DataContext.FavouriteTracks.All(tracks => tracks != playlist))
            .Include(playlist => playlist.Picture)
            .AsNoTracking()
            .Select(playlist => new Playlist
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Picture = playlist.Picture,
                IsAlbum = playlist.IsAlbum,
                IsLiked = IsSignedIn
                    ? playlist.Users!.Any(user => user.Id == CurrentUserId)
                    : null,
                IsOwned = IsSignedIn
                    ? CurrentUserQueryable
                        .Select(user => user.OwnedPlaylists!.Playlists!)
                        .Any(playlists => playlists.Any(playlist1 => playlist1 == playlist))
                    : null
            })
            .ToListAsync());

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult LikePlaylist(int id) =>
        _playlistsRepo.LikePlaylist(User, id)
            ? Ok()
            : BadRequest();

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult UnlikePlaylist(int id) =>
        _playlistsRepo.UnlikePlaylist(User, id)
            ? Ok()
            : BadRequest();

    [HttpGet, OpenIdDictAuthorize]
    public async Task<IActionResult> Liked()
    {
        CurrentUser = await CurrentUserQueryable
            .Include(user => user.Playlists!)
            .ThenInclude(playlist => playlist.Picture)
            .AsNoTracking()
            .FirstAsync();
        CurrentUser.Playlists!.ForEach(playlist => playlist.IsLiked = true);
        return Json(new
        {
            User = new
            {
                CurrentUser.Id,
                CurrentUser.Picture,
                CurrentUser.Email,
                CurrentUser.UserName
            },
            Playlists = CurrentUser.Playlists.Select(playlist => new
            {
                playlist.Id,
                playlist.Name,
                playlist.IsLiked,
                playlist.Picture
            })
        });
    }

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult Create([FromForm] CreatePlaylistViewModel model) =>
        ModelState.IsValid
            ? RedirectToAction("Index",
                new {_playlistsRepo.CreatePlaylist(User, model.Name, model.Picture.ToPicture()).Id})
            : BadRequest();

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult DeleteSure(int id) =>
        _playlistsRepo.DeletePlaylist(User, id)
            ? Ok()
            : BadRequest();

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult AddTrackToPlaylist(int playlistId, int trackId) =>
        _playlistsRepo.AddTrackToPlaylist(User, playlistId, trackId)
            ? Ok()
            : BadRequest();

    [HttpPost, OpenIdDictAuthorize]
    public IActionResult RemoveTrackFromPlaylist(int playlistId, int trackId) =>
        _playlistsRepo.RemoveTrackFromPlaylist(User, playlistId, trackId)
            ? Ok()
            : BadRequest();

    [HttpGet]
    public IActionResult GetData(int id) =>
        _playlistsRepo.GetData(id) switch
        {
            { } playlist => Json(playlist),
            _ => NotFound()
        };
}
