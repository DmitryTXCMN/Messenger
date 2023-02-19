namespace Planscam.FsServices

open Microsoft.AspNetCore.Identity
open Microsoft.EntityFrameworkCore
open Microsoft.FSharp.Core
open Planscam.DataAccess
open Planscam.Entities
open System.Linq

type PlaylistsRepo(dataContext: AppDbContext, userManager: UserManager<User>, signInManager: SignInManager<User>) =

    let userId user = userManager.GetUserId user
    let mutable _userQueryable = None

    let userQueryable userPrincipal =
        match _userQueryable with
        | Some r -> r
        | None ->
            let value =
                dataContext.Users.Where(fun user -> user.Id = userId userPrincipal)

            _userQueryable <- Some(value)
            value

    member _.GetLikedPlaylists(userPrincipal) =
        (query{
            for playlist in dataContext.Playlists do
                where (playlist.Users.Contains(userQueryable(userPrincipal).First()))
                select playlist
        })
            .Include(fun playlist -> playlist.Picture)
            .ToListAsync()

    member _.GetFavouriteTracksId(userPrincipal) =
        userQueryable(userPrincipal)
            .AsNoTracking()
            .Select(fun user -> user.FavouriteTracks.Id)
            .FirstAsync()

    member _.GetPlaylistFull(id, userPrincipal) =
        (if signInManager.IsSignedIn(userPrincipal) then
             (query {
                 for playlist in dataContext.Playlists do
                     where (playlist.Id = id)

                     select (
                         Playlist(
                             Id = playlist.Id,
                             Name = playlist.Name,
                             Picture = playlist.Picture,
                             Tracks =
                                 (query {
                                     for track in playlist.Tracks do
                                         select (
                                             Track(
                                                 Id = track.Id,
                                                 Name = track.Name,
                                                 Picture = track.Picture,
                                                 Author = track.Author,
                                                 IsLiked =
                                                     (query {
                                                         for user in userQueryable userPrincipal do
                                                             select (user.FavouriteTracks.Tracks.Contains(track))
                                                      })
                                                         .First()
                                             )
                                         )
                                  })
                                     .ToList(),
                             IsLiked =
                                 (query {
                                     for user in userQueryable userPrincipal do
                                         select (user.Playlists.Any(fun p -> p = playlist))
                                  })
                                     .First(),
                             IsOwned =
                                 (query {
                                     for user in userQueryable userPrincipal do
                                         select (user.OwnedPlaylists.Playlists.Any(fun p -> p = playlist))
                                  })
                                     .First()
                         )
                     )
              })
         else
             (query {
                 for playlist in dataContext.Playlists do
                     where (playlist.Id = id)

                     select (
                         Playlist(
                             Id = playlist.Id,
                             Name = playlist.Name,
                             Picture = playlist.Picture,
                             Tracks =
                                 (query {
                                     for track in playlist.Tracks do
                                         select (
                                             Track(
                                                 Id = track.Id,
                                                 Name = track.Name,
                                                 Picture = track.Picture,
                                                 Author = track.Author
                                             )
                                         )
                                  })
                                     .ToList()
                         )
                     )
              }))
            .AsNoTracking()
            .FirstOrDefaultAsync()

    member _.LikePlaylist(userPrincipal, id) =
        match (query {
                   for playlist in dataContext.Playlists do
                       where (playlist.Id = id)
               })
            .FirstOrDefault() with
        | null -> false
        | playlist ->
            userQueryable(userPrincipal)
                .Include(fun user -> user.Playlists.Where(fun _ -> false))
                .First()
                .Playlists.Add(playlist)

            dataContext.SaveChanges() |> ignore
            true

    member _.UnlikePlaylist(userPrincipal, id) =
        let user =
            userQueryable(userPrincipal)
                .Include(fun user -> user.Playlists.Where(fun playlist -> playlist.Id = id))
                .First()

        if user.Playlists.Any() then
            user.Playlists.Clear()
            dataContext.SaveChanges() |> ignore
            true
        else
            false

    member _.CreatePlaylist(userPrincipal, name, picture) =
        let user =
            (query {
                for user in
                    userQueryable(userPrincipal)
                        .Include(fun user -> user.OwnedPlaylists.Playlists)
                        .Include(fun user -> user.Playlists) do
                    select user
             })
                .First()

        let playlist =
            Playlist(Name = name, Picture = picture)
        user.OwnedPlaylists.Playlists.Add playlist
        dataContext.SaveChanges() |> ignore
        playlist

    member _.DeletePlaylist(userPrincipal, id) =
        match (query{
                for playlist in dataContext.Playlists do
                    where(playlist.Id = id &&
                          (query{
                              for user in userQueryable userPrincipal do
                                  where (user.OwnedPlaylists.Playlists.Contains(playlist))
                          }).Any())
                    select playlist
            })
                .FirstOrDefault() with
        | null -> false
        | playlist ->
            dataContext.Playlists.Remove playlist |> ignore
            dataContext.SaveChanges() |> ignore
            true

    member _.AddTrackToPlaylist(userPrincipal, playlistId, trackId) =
        let playlist =
            dataContext
                .Playlists
                .Include(fun p -> p.Tracks)
                .FirstOrDefault(fun p -> p.Id = playlistId)
        
        if playlist = null
           || playlist.Tracks.Any(fun t -> t.Id = trackId) //не добавлен ли уже этот трек в плейлист
           || true <> userQueryable(userPrincipal)
               .Select(fun u -> u.OwnedPlaylists.Playlists)
               .Any(fun p -> p.Contains(playlist)) then //принадлежит ли плейлист юзеру
           false
        else
            match dataContext.Tracks.FirstOrDefault(fun t -> t.Id = trackId) with //существует ли трек
            | null -> false
            | track ->
                playlist.Tracks.Add(track)
                dataContext.SaveChanges() |> ignore
                true

    member _.RemoveTrackFromPlaylist(userPrincipal, playlistId, trackId) =
        let playlist =
            dataContext
                .Playlists
                .Include(fun p -> p.Tracks)
                .FirstOrDefault(fun p -> p.Id = playlistId)

        if playlist = null
           || playlist.Tracks.All(fun t -> t.Id <> trackId) //добавлен ли этот трек в плейлист
           || true <> userQueryable(userPrincipal)
               .Select(fun u -> u.OwnedPlaylists.Playlists)
               .Any(fun p -> p.Contains(playlist)) then //принадлежит ли плейлист юзеру
            false
        else
            match dataContext.Tracks.FirstOrDefault(fun t -> t.Id = trackId) with //существует ли трек
            | null -> false
            | track ->
                playlist.Tracks.Remove(track) |> ignore
                dataContext.SaveChanges() |> ignore
                true

    member _.GetData(id) =
        (query {
            for playlist in dataContext.Playlists do
                where (playlist.Id = id)

                select
                    {| Id = playlist.Id
                       Name = playlist.Name
                       TrackIds = playlist.Tracks.Select(fun track -> track.Id) |}
         })
            .FirstOrDefault()
            
    member _.GetOwnedPlaylists(userPrincipal) = 
        userQueryable(userPrincipal)
            .Select(fun user -> user.OwnedPlaylists.Playlists)
            .FirstAsync()