using System.ComponentModel.DataAnnotations.Schema;

namespace Planscam.Entities;

[Table(nameof(FavouriteTracks))]
public class FavouriteTracks : Playlist
{
    public FavouriteTracks() : this("Fav tracks") { }

    public FavouriteTracks(string name) =>
        Name = name;
}
