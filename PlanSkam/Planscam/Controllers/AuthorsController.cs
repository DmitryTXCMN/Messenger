using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.Models;

namespace Planscam.Controllers;

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
                { } author => View(author),
                _ => NotFound()
            };

    [HttpGet]
    public IActionResult Search(AuthorsSearchViewModel? model)
    {
        if (model is null || !ModelState.IsValid) return View(model);
        model.Result = _authorsRepo.SearchAuthors(model.Query, model.Page);
        return View(model);
    }
}
