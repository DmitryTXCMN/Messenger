using Microsoft.AspNetCore.Identity;
using Planscam.DataAccess;
using Planscam.Entities;

namespace Planscam.Middleware;

public class SubscriptionsMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionsMiddleware(RequestDelegate next) =>
        _next = next;

    public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, AppDbContext dataContext)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is {SubExpires: { } subExpires} && subExpires > DateTime.Now)
        {
            user.SubExpires = null;
            await userManager.RemoveFromRoleAsync(user, "Sub");
            await dataContext.SaveChangesAsync();
        }

        await _next(context);
    }
}
