using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Extensions;
using Planscam.Models;

namespace Planscam.Controllers;

public class ProfileController : PsmControllerBase
{
    public ProfileController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager)
        : base(dataContext, userManager, signInManager)
    {
    }

    private UserViewModel GetModel(User user) =>
        new()
        {
            Id = user.Id, Name = user.UserName, Email = user.Email, Picture = user.Picture,
            IsAuthor = User.IsInRole("Author"),
            SubExpires = user.SubExpires
        };

    //я тут ультанул
    [HttpGet]
    public async Task<IActionResult> Index(string? id) =>
        (id, SignInManager.IsSignedIn(User)) switch
        {
            (null, false) => Unauthorized(),
            (null, true) => View(GetModel(
                await CurrentUserQueryable
                    .Include(user => user.Picture)
                    .AsNoTracking()
                    .FirstAsync())),
            ({ }, _) => await DataContext.Users
                    .Where(user => user.Id == id)
                    .Include(user => user.Picture)
                    .AsNoTracking()
                    .FirstOrDefaultAsync() switch
                {
                    { } userById => View(GetModel(userById)),
                    _ => NotFound()
                }
        };

    [HttpGet, Authorize]
    public async Task<IActionResult> Edit() =>
        View(GetModel(await CurrentUserQueryable
            .Include(user => user.Picture)
            .AsNoTracking()
            .FirstAsync()));

    [HttpPost, Authorize]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        var user = model.UploadImage is { }
            ? await CurrentUserQueryable.FirstAsync()
            : await CurrentUserQueryable.Include(user => user.Picture).FirstAsync();
        if (model.UploadImage is { })
            user.Picture = model.UploadImage.ToPicture();
        user.UserName = model.Name;
        user.Email = model.Email;
        await UserManager.UpdateAsync(user);
        model.Picture = user.Picture;
        return View("CloseAndRedict","Profile/Index");
    }
}
