using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;

namespace Planscam.Controllers;

[Authorize]
public class SubscriptionsController : PsmControllerBase
{
    public SubscriptionsController(AppDbContext dataContext, UserManager<User> userManager,
        SignInManager<User> signInManager) : base(dataContext, userManager, signInManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index() =>
        View(await DataContext.Subscriptions.ToListAsync());

    [HttpGet]
    public async Task<IActionResult> PaymentEmulator(int id) =>
        await DataContext.Subscriptions.FindAsync(id) switch
        {
            { } sub => View(sub),
            _ => NotFound()
        };

    [HttpPost]
    public async Task<IActionResult> BuySub(int id)
    {
        var sub = await DataContext.Subscriptions.FindAsync(id);
        if (sub is null) return BadRequest();
        await UserManager.AddToRoleAsync(CurrentUser, "SUB");
        CurrentUser.SubExpires = DateTime.Now + Subscription.SubscriptionDurationToTimeSpan(sub.Duration);
        await DataContext.SaveChangesAsync();
        return View("CloseAndReload");
    }

    [HttpPost]
    public async Task<IActionResult> UnSub()
    {
        if (!await UserManager.IsInRoleAsync(CurrentUser, "SUB"))
            return BadRequest();
        await UserManager.RemoveFromRoleAsync(CurrentUser, "SUB");
        CurrentUser.SubExpires = null;
        await UserManager.UpdateAsync(CurrentUser);
        return Ok();
    }
}
