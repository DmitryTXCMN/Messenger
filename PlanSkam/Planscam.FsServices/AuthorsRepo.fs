namespace Planscam.FsServices

open Microsoft.AspNetCore.Identity
open Microsoft.EntityFrameworkCore
open Microsoft.FSharp.Core
open Planscam.DataAccess
open Planscam.Entities
open System.Linq

type AuthorsRepo(dataContext: AppDbContext, userManager: UserManager<User>) =
    member _.SearchAuthors(q: string, page) =
        (query {
            for author in dataContext.Authors do
                where (author.Name.Contains(q))
                skip (10 * (if page = 0 then 0 else (page - 1)))
                take 10
         })
            .Include(fun author -> author.Picture)
            .ToList()
