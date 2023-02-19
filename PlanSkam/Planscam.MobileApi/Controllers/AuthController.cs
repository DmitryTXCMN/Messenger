using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.MobileApi.Models;

namespace Planscam.MobileApi.Controllers;

public class AuthController : PsmControllerBase
{
    private readonly UsersRepo _usersRepo;

    public AuthController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager,
        UsersRepo usersRepo) :
        base(dataContext, userManager, signInManager) =>
        _usersRepo = usersRepo;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var user = _usersRepo.CreateNewUser(model.UserName, model.Email);
        var result = await UserManager.CreateAsync(user, model.Password);
        return result.Succeeded
            ? Ok()
            : Forbid(new AuthenticationProperties(result.Errors.ToDictionary(error => error.Code,
                error => error.Description)!));
    }

    [HttpPost, Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Login([FromForm] LoginViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new Exception("OpenIdDict config is wrong");
        if (await UserManager.FindByNameAsync(model.username) is not { } user)
            return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "the username is incorrect"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!(await SignInManager.CheckPasswordSignInAsync(user, model.password, lockoutOnFailure: true)).Succeeded)
            return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "the password is incorrect"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var userPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
        userPrincipal.SetScopes(new[]
        {
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        }.Intersect(request.GetScopes()));
        foreach (var claim in userPrincipal.Claims)
            claim.SetDestinations(GetDestinations(claim, userPrincipal));
        return SignIn(userPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Name:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Email:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Role:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case "AspNet.Identity.SecurityStamp":
                    yield break;
                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logoff()
    {
        await SignInManager.SignOutAsync();
        return Ok();
    }
}
