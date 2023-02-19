using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Planscam.Entities;

public class User : IdentityUser
{
    public Picture? Picture { get; set; }

    /// <summary>
    /// liked playlists
    /// </summary>
    [Required]
    public List<Playlist>? Playlists { get; set; }

    /// <summary>
    /// special playlist, contains liked tracks
    /// </summary>
    [Required]
    public FavouriteTracks? FavouriteTracks { get; set; }

    /// <summary>
    /// only owned by this user, can be not in <see cref="Playlists"/>
    /// </summary>
    [Required]
    public OwnedPlaylists? OwnedPlaylists { get; set; }
    
    /// <summary>
    /// shows when users subscription inspires
    /// null if user has no subscription
    /// </summary>
    public DateTime? SubExpires { get; set; }
}
