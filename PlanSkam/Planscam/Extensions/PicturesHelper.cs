using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planscam.Entities;

namespace Planscam.Extensions;

public static class PicturesHelper
{
    public static IHtmlContent DrawUserAvatar(this IHtmlHelper helper, Picture? picture, string? @class = default) =>
        helper.DrawPic(picture, 80, default, @class);

    public static IHtmlContent DrawSmallTrackPic(this IHtmlHelper helper, Picture? picture, string? @class = default) =>
        helper.DrawPic(picture, 40, default, @class);

    public static IHtmlContent DrawHugeTrackPic(this IHtmlHelper helper, Picture? picture, string? @class = default) =>
        helper.DrawPic(picture, 500, default, @class);

    public static IHtmlContent DrawSmallPlaylistPic(this IHtmlHelper helper, Picture? picture,
        string? @class = default) =>
        helper.DrawPic(picture, 40, default, @class);

    public static IHtmlContent DrawHugePlaylistPic(this IHtmlHelper helper, Picture? picture,
        string? @class = default) =>
        helper.DrawPic(picture, 700, default, @class);

    public static IHtmlContent DrawSmallAuthorPic(this IHtmlHelper helper, Picture? picture,
        string? @class = default) =>
        helper.DrawPic(picture, 50, default, @class);

    private static IHtmlContent DrawPic(this IHtmlHelper helper, Picture? picture, int x, int? y = default,
        string? @class = default) =>
        helper.Raw($"<img style='width:{x}px; height:{y ?? x}px;' class=\"{@class}\" src=\"{CreateSrc(picture)}\"/>");

    private static string CreateSrc(Picture? picture) =>
        picture?.Data.ToPictureString() ?? "/img/vopros.jpg";

    private static string ToPictureString(this byte[] arr) =>
        $"data:image/jpeg;base64,{Convert.ToBase64String(arr)}";
}
