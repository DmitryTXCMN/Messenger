using Planscam.Entities;

namespace Planscam.MobileApi;

public static class PicturesExtensions
{
    public static Picture? ToPicture(this IFormFile? formFile)
    {
        if (formFile is null) return null;
        using var reader = new BinaryReader(formFile.OpenReadStream());
        return new Picture {Data = reader.ReadBytes((int) formFile.Length)};
    }
}
