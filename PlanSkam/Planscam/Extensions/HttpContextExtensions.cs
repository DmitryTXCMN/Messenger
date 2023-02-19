namespace Planscam.Extensions;

public static class HttpContextExtensions
{
    public static string GetCurrentUrl(this HttpContext httpContext) =>
        $"{httpContext.Request.Path}{httpContext.Request.QueryString}";
}
