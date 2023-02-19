using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Models;

namespace Planscam.Controllers;

[Authorize(Roles = "Author")]
public class StudioController : PsmControllerBase
{
    public StudioController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager)
        : base(dataContext, userManager, signInManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var author = await DataContext.Authors
            .Include(author => author.Tracks)
            .Include(author => author.Picture)
            .FirstAsync(author => author.User == CurrentUser);
        author.Tracks = DataContext.Tracks
            .Where(track => author.Tracks.Contains(track))
            .Select(track => new Track
            {
                Id = track.Id,
                Name = track.Name,
                Picture = track.Picture,
                Author = track.Author,
                IsLiked = CurrentUserQueryable.Select(user => user.FavouriteTracks!.Tracks!.Contains(track)).First()
            })
            .ToList();
        return View(author);
    }

    [HttpGet]
    public async Task<IActionResult> MyTracks() =>
        View(await DataContext.Tracks
            .Where(track =>
                track.Author == DataContext.Authors.First(author => author.User == CurrentUserQueryable.First()))
            .ToListAsync());

    [HttpGet]
    public IActionResult LoadNewTrack() =>
        View(new LoadTrackViewModel
        {
            Genres = DataContext.Genres.ToList()
        });

    [HttpPost]
    public async Task<IActionResult> LoadNewTrack(LoadTrackViewModel model)
    {
        model.Genres = DataContext.Genres.ToList();
        if (!ModelState.IsValid) return View(model);
        var data = new TrackData();
        using (var reader = new BinaryReader(model.Track!.OpenReadStream()))
        {
            data.Data = reader.ReadBytes((int) model.Track.Length);
        }

        var track = new Track
        {
            Name = model.Name!,
            Data = data,
            Author = await DataContext.Authors.FirstAsync(author => author.User == CurrentUserQueryable.First()),
            Genre = await DataContext.Genres.FirstAsync(genre => genre.Id == model.GenreId)
        };
        if (model.Image is { })
        {
            track.Picture = new Picture();
            using var reader = new BinaryReader(model.Image.OpenReadStream());
            track.Picture.Data = reader.ReadBytes((int) model.Image.Length);
        }

        await DataContext.Tracks.AddAsync(track);
        await DataContext.SaveChangesAsync();
        return View("CloseAndRedict", $"/Tracks/Index/{track.Id}");
    }

    private IQueryable<Track> GetOwnTrackById(int id, bool includePic = false) =>
        (includePic
            ? DataContext.Tracks.Include(track => track.Picture)
            : DataContext.Tracks as IQueryable<Track>)
        .Where(track =>
            track.Id == id &&
            track.Author == DataContext.Authors.First(author => author.User == CurrentUserQueryable.First()));

    [HttpGet]
    public async Task<IActionResult> DeleteTrack(int id, string? returnUrl) =>
        await GetOwnTrackById(id, true)
                .FirstOrDefaultAsync() switch
            {
                { } track => View(new DeleteTrackViewModel
                {
                    Track = track,
                    ReturnUrl = returnUrl
                }),
                _ => NotFound()
            };

    [HttpPost]
    public async Task<IActionResult> DeleteTrackSure(int id, string? returnUrl)
    {
        var track = await GetOwnTrackById(id)
            .Select(track => new Track
            {
                Id = track.Id,
                Data = new TrackData
                {
                    Id = track.Data!.Id
                }
            })
            .FirstOrDefaultAsync();
        if (track is null) return BadRequest();
        DataContext.TrackDatas.Remove(track.Data!);
        DataContext.Tracks.Remove(track);
        await DataContext.SaveChangesAsync();
        return IsLocalUrl(returnUrl)
            ? Redirect(returnUrl!)
            : RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Albums() =>
        View(await CurrentUserQueryable
            .Select(user => user.OwnedPlaylists!.Playlists!.Where(playlist => playlist.IsAlbum).ToList())
            .FirstAsync());

    [HttpGet]
    public IActionResult CreateAlbum() =>
        View(); //todo
}
