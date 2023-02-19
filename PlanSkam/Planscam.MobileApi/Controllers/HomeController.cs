using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

public class HomeController : PsmControllerBase
{
    public HomeController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager) :
        base(dataContext, userManager, signInManager)
    {
    }

    //todo переписать полностью
    [HttpGet, OpenIdDictAuthorize, AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var playlists = await DataContext.Playlists
            .Include(playlist => playlist.Picture)
            .AsNoTracking()
            .ToListAsync();
        if (!IsSignedIn) return Json(new HomePageViewModel {Playlists = playlists});
        CurrentUser = await CurrentUserQueryable
            .Include(user => user.Picture)
            .Include(user => user.FavouriteTracks)
            .Include(user => user.FavouriteTracks!.Picture)
            .AsNoTracking()
            .FirstAsync();
        CurrentUser.FavouriteTracks!.Tracks = DataContext.Tracks
            .Where(track => track.Playlists!.Contains(CurrentUser.FavouriteTracks!))
            .Include(track => track.Picture)
            .AsNoTracking()
            .ToList();
        return Json(new HomePageViewModel
        {
            Playlists = playlists,
            User = CurrentUser
        });
    }
}
