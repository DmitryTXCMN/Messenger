using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;

namespace Planscam.Controllers;

public class TestController : PsmControllerBase
{
    private readonly UsersRepo _usersRepo;

    public TestController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager,
        UsersRepo usersRepo) :
        base(dataContext, userManager, signInManager)
    {
        _usersRepo = usersRepo;
    }

    public async Task<IActionResult> AddTestEntities()
    {
        if (DataContext.Authors.Any())
            return Ok();
        var genrePic = new Picture
        {
            Data = Array.Empty<byte>()
        };
        var genre = new Genre
        {
            Name = "test genre",
            Picture = genrePic,
        };
        var user = _usersRepo.CreateNewUser("qwe", "qwe@qwe.qwe");
        user.FavouriteTracks!.Tracks = new List<Track>();
        await UserManager.CreateAsync(user, "qweQWE123!");
        var author = new Author {Name = "Author1", User = user};
        var tracks = new List<Track>();
        for (var i = 0; i < 10; i++)
            tracks.Add(new Track
            {
                Name = $"track{i + 1}",
                Data = new TrackData
                {
                    Data = Array.Empty<byte>()
                },
                Author = author,
                Genre = genre
            });
        await DataContext.Pictures.AddAsync(genrePic);
        await DataContext.SaveChangesAsync();
        await DataContext.Genres.AddAsync(genre);
        await DataContext.SaveChangesAsync();
        await DataContext.Authors.AddAsync(author);
        await DataContext.SaveChangesAsync();
        await DataContext.Tracks.AddRangeAsync(tracks);
        await DataContext.SaveChangesAsync();
        user.FavouriteTracks.Tracks.AddRange(tracks.GetRange(0, 6));
        await DataContext.SaveChangesAsync();
        await UserManager.AddToRoleAsync(user, "Author");
        await DataContext.Playlists.AddAsync(new Playlist
        {
            Name = "test playlist",
            Tracks = tracks.GetRange(4, 3)
        });
        await DataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        foreach (var playlist in CurrentUserQueryable.Include(user => user.OwnedPlaylists.Playlists).First()
                     .OwnedPlaylists.Playlists)
        {
            Console.WriteLine($"playlist {playlist.Id} {playlist.Name}");
        }

        return Ok();
    }
}
