import {EntityRepository, Repository, getRepository} from "typeorm";
import {User} from "../Entities/User";
import {Role} from "../Entities/Role";
import {Playlist} from "../Entities/Playlist";
import {Author} from "../Entities/Author";
import {Track} from "../Entities/Track";

@EntityRepository(User)
export class UsersRepository extends Repository<User> {
    async GetUser(id: string){
        const user = await this.findOne(id);
        console.log(user);
        return user;
    }
    
    async GetAll() {
        return await this.find({relations: ["Playlists"]});
    }

    async IsAuthor(id: string): Promise<boolean> {
        return (Boolean)((await this.createQueryBuilder('user')
            .where("user.Id = :id", {id})
            .innerJoin("user.Roles", "roles", "roles.Name = 'Author'")
            .select("case when count(*) > 0 then 1 else 0 end as isAuthor")
            .getRawOne<object>())["isAuthor"]);
    }

    async MakeAuthor(id: string) {
        const rolesRepo = getRepository(Role);
        const role = await rolesRepo
            .createQueryBuilder("role")
            .where("role.Name = 'Author'")
            .getOne();
        if (role == null)
            return false;
        const user = await this.findOne(id, {relations: ["Roles"]});
        if (user == null)
            return false;
        user.Roles.push(role);
        await this.save(user);

        const authorsRepo = getRepository(Author);
        const author = await authorsRepo
            .createQueryBuilder("author")
            .where("author.UserId = :id", {id})
            .getOne();
        if (author == null) {
            await this.createAuthor(user);
        }
        return true;
    }

    async createAuthor(user: User) {
        const authorsRepo = getRepository(Author);
        let author = new Author();
        author.UserId = user.Id;
        author.Name = user.UserName;
        await authorsRepo.save(author);
    }

    async MakeNotAuthor(id: string) {
        const rolesRepo = getRepository(Role);
        const role = await rolesRepo
            .createQueryBuilder("role")
            .where("role.Name = 'Author'")
            .getOne();
        if (role == null)
            return false;
        const user = await this.findOne(id, {relations: ["Roles"]});
        if (user == null)
            return false;
        user.Roles.splice(user.Roles.indexOf(role), 1);
        await this.save(user);
        return true;
    }

    async getFavTracks(id: string) {
        return await this.createQueryBuilder("users")
            .innerJoin("FavouriteTracks", "FT", "users.FavouriteTracksId = FT.Id")
            .innerJoin("Playlists", "P", "FT.Id = P.Id")
            .innerJoin("PlaylistTrack", "PT", "P.Id = PT.PlaylistsId")
            .innerJoin("Tracks", "T", "PT.TracksId = T.Id")
            .where("users.Id = :id", {id})
            .select("T.*")
            .getRawMany<Track>();
    }

    async addPlaylistToLiked(userId: string, playlistId: number) {
        const playlistsRepo = getRepository(Playlist);
        const playlist = await playlistsRepo.findOne(playlistId);
        if (playlist == null)
            return false;
        const user = await this.findOne(userId, {relations: ["Playlists"]});
        if (user == null)
            return false;
        user.Playlists.push(playlist);
        await this.save(user);
        return true;
    }

    async removePlaylistFromLiked(userId: string, playlistId: number) {
        const playlistsRepo = getRepository(Playlist);
        const playlist = await playlistsRepo.findOne(playlistId);
        if (playlist == null)
            return false;
        const user = await this.findOne(userId, {relations: ["Playlists"]});
        if (user == null)
            return false;
        user.Playlists.splice(user.Playlists.indexOf(playlist), 1);
        await this.save(user);
        return true;
    }

    async changeEmail(userId: string, email: string) {
        const user = await this.findOne(userId);
        if (user == null)
            return false;
        user.Email = email;
        user.NormalizedEmail = email.toUpperCase();
        await this.save(user);
        return true;
    }
    
    async addTrackToFavourites(userId: string, trackId: number){
        const user = await this.findOne(userId);
        if (user == null)
            return false;
        await this.query(`insert into PlaylistTrack values (${user.FavouriteTracksId}, ${trackId})`);
        return true;
    }

    async removeTrackFromFavourites(userId: string, trackId: number){
        const user = await this.findOne(userId);
        if (user == null)
            return false;
        return await this.query(`delete from PlaylistTrack where PlaylistsId=${user.FavouriteTracksId} and TracksId=${trackId}`) === 1;
    }
}