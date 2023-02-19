using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Planscam.Extensions;

public static class ReturnUrlHelper
{
    /// <summary>
    /// юзает IHtmlHelper.HiddenFor, поэтому багованный
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IHtmlContent HiddenForReturnUrl<T>(this IHtmlHelper<T> helper, HttpContext context)
    {
        var returnUrl = context.GetCurrentUrl();
        return helper.HiddenFor(_ => returnUrl);
    }
}
