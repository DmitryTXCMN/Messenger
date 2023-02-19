using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.Extensions;

namespace Planscam.Controllers;

public abstract class PsmControllerBase : Controller
{
    protected readonly AppDbContext DataContext;
    protected readonly UserManager<User> UserManager;
    protected readonly SignInManager<User> SignInManager;

    private User? _currentUser;

    /// <summary>
    /// при первом получении долбит бд
    /// </summary>
    protected User CurrentUser
    {
        get => _currentUser ??= UserManager.GetUserAsync(User).Result;
        set => _currentUser = value;
    }

    private string? _currentUserId;

    /// <summary>
    /// работает быстро
    /// </summary>
    protected string CurrentUserId =>
        _currentUserId ??= UserManager.GetUserId(User);

    private IQueryable<User>? _currentUserQueryable;

    protected IQueryable<User> CurrentUserQueryable =>
        _currentUserQueryable ??= DataContext.Users.Where(user => user.Id == CurrentUserId);

    protected PsmControllerBase(AppDbContext dataContext, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        DataContext = dataContext;
        UserManager = userManager;
        SignInManager = signInManager;
    }

    protected bool IsLocalUrl(string? url) =>
        !string.IsNullOrEmpty(url) && Url.IsLocalUrl(url);

    [NonAction]
    protected new IActionResult Unauthorized() =>
        RedirectToAction("Login", "Auth", new {returnUrl = HttpContext.GetCurrentUrl()});
}
