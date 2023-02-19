using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.Models;

namespace Planscam.Controllers;

public class AuthController : PsmControllerBase
{
    private readonly UsersRepo _usersRepo;

    public AuthController(AppDbContext dataContext, UserManager<User> userManager, SignInManager<User> signInManager,
        UsersRepo usersRepo) :
        base(dataContext, userManager, signInManager) =>
        _usersRepo = usersRepo;

    [HttpGet]
    public IActionResult Register() =>
        View();

    [HttpGet]
    public IActionResult Login(string? returnUrl) =>
        View(new LoginViewModel {ReturnUrl = returnUrl});

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var (name, email, pass) = model;
        var user = _usersRepo.CreateNewUser(name, email);
        var result = await UserManager.CreateAsync(user, pass);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View();
        }

        await SignInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View();
        if ((await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false))
            .Succeeded)
            return IsLocalUrl(model.ReturnUrl) ? Redirect(model.ReturnUrl!) : RedirectToAction("Index", "Home");
        ModelState.AddModelError(string.Empty, "wrong email or password");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logoff()
    {
        await SignInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    public IActionResult GoogleLogin(string provider, string returnUrl = "")
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth",
            new {ReturnUrl = returnUrl});

        var properties =
            SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        var loginViewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        };

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Ошибка со внешнего провайдера: {remoteError}");
            return View("Login", loginViewModel);
        }

        // Get the login information about the user from the external login provider
        var info = await SignInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState.AddModelError(string.Empty, "Ошибка при загрузке внешней информации для входа.");
            return View("Login", loginViewModel);
        }

        // If the user already has a login (i.e if there is a record in AspNetUserLogins
        // table) then sign-in the user with this external login provider
        var signInResult = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider,
            info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        // If there is no record in AspNetUserLogins table, the user may not have
        // a local account
        else
        {
            // Get the email claim value
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                // Create a new user without password if we do not have a user already
                var user = await UserManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = _usersRepo.CreateNewUser(info.Principal.FindFirstValue(ClaimTypes.GivenName),
                        info.Principal.FindFirstValue(ClaimTypes.Email));
                    await UserManager.CreateAsync(user);
                }

                // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                await UserManager.AddLoginAsync(user, info);
                await SignInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            // If we cannot find the user email we cannot continue
            ViewBag.ErrorTitle = $"Email не получен со внешнего провайдера: {info.LoginProvider}";
            ViewBag.ErrorMessage = "Пожалуйста, обратитесь к нам на почту: support@mybook.ru";

            return View("Error");
        }
    }

    [Route("signin-google")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        {
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Type,
            claim.Value
        });
        return Json(claims);
    }
}
