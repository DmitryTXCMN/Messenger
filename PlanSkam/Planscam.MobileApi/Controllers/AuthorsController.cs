using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

public class AuthorsController : PsmControllerBase
{
    private readonly AuthorsRepo _authorsRepo;

    public AuthorsController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager,
        AuthorsRepo authorsRepo)
        : base(dataContext, userManager, signInManager) =>
        _authorsRepo = authorsRepo;

    [HttpGet]
    public async Task<IActionResult> Index(int id) =>
        await DataContext.Authors
                .Select(a => new AuthorPageViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Picture = a.Picture!,
                    Albums = a.User!.Playlists!.Where(p => p.IsAlbum).ToList(),
                    RecentReleases = new Playlist
                    {
                        Name = $"{a.Name}'s last releases",
                        Tracks = a.Tracks!.OrderByDescending(track => track.Id).Take(20).ToList()
                    }
                })
                .FirstOrDefaultAsync(a => a.Id == id) switch
            {
                { } author => Json(author),
                _ => NotFound()
            };

    [HttpGet]
    public IActionResult Search(string query, int page) =>
        Json(_authorsRepo.SearchAuthors(query, page));
}
