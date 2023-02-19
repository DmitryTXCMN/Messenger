using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Planscam.MobileApi;

public class OpenIdDictAuthorize : AuthorizeAttribute
{
    public OpenIdDictAuthorize() => 
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}
