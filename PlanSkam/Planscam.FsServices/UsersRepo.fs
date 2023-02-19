namespace Planscam.FsServices

open Planscam.Entities

type UsersRepo() =
    member this.CreateNewUser(name : string, email : string) : User =
        let user = User()
        user.UserName <- name
        user.Email <- email
        user.FavouriteTracks <- FavouriteTracks($"{name}'s favorite tracks")
        user.OwnedPlaylists <- OwnedPlaylists()
        user