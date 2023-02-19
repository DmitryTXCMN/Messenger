using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Planscam.Entities;

namespace Planscam.DataAccess;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<FavouriteTracks> FavouriteTracks { get; set; } = null!;
    public DbSet<Picture> Pictures { get; set; } = null!;
    public DbSet<Playlist> Playlists { get; set; } = null!;
    public DbSet<Track> Tracks { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<TrackData> TrackDatas { get; set; } = null!;
    public DbSet<OwnedPlaylists> OwnedPlaylists { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder
            .CreateRoles()
            .CreateSubscriptions();
    }
}
