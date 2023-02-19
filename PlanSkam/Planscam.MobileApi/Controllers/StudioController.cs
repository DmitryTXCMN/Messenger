using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

[OpenIdDictAuthorize(Roles = "Author")]
public class StudioController : PsmControllerBase
{
    public StudioController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager)
        : base(dataContext, userManager, signInManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> MyTracks() =>
        Json(await DataContext.Tracks
            .Where(track =>
                track.Author == DataContext.Authors.First(author => author.User == CurrentUserQueryable.First()))
            .ToListAsync());

    [HttpPost]
    public async Task<IActionResult> LoadNewTrack([FromForm]LoadTrackViewModel model)
    {
        model.Genres = DataContext.Genres.ToList();
        if (!ModelState.IsValid) return BadRequest();
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
        return Json(model);
    }

    private IQueryable<Track> GetOwnTrackById(int id, bool includePic = false) =>
        (includePic
            ? DataContext.Tracks.Include(track => track.Picture)
            : DataContext.Tracks as IQueryable<Track>)
        .Where(track =>
            track.Id == id &&
            track.Author == DataContext.Authors.First(author => author.User == CurrentUserQueryable.First()));


    [HttpPost]
    public async Task<IActionResult> DeleteTrackSure(int id)
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
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Albums() =>
        Json(await CurrentUserQueryable
            .Select(user => user.OwnedPlaylists!.Playlists!.Where(playlist => playlist.IsAlbum).ToList())
            .FirstAsync());
}
