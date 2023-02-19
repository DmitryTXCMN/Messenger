using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

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
            IsAuthor = User.IsInRole("Author")
        };

    [HttpGet, OpenIdDictAuthorize, AllowAnonymous]
    public async Task<IActionResult> Index(string? id) =>
        (id, true) switch
        {
            (null, false) => Unauthorized(),
            (null, true) => Json(GetModel(
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
                    { } userById => Json(GetModel(userById)),
                    _ => NotFound()
                }
        };

    [HttpPost, OpenIdDictAuthorize]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        var user = model.UploadImage is { }
            ? await CurrentUserQueryable.FirstAsync()
            : await CurrentUserQueryable.Include(user => user.Picture).FirstAsync();
        if (model.UploadImage is { })
            user.Picture = model.UploadImage.ToPicture();
        user.UserName = model.Name;
        await UserManager.UpdateAsync(user);
        model.Picture = user.Picture;
        return Json(model);
    }
}
