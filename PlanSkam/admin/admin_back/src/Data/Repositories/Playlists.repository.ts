import {EntityRepository, Repository} from "typeorm";
import {Playlist} from "../Entities/Playlist";

@EntityRepository(Playlist)
export class PlaylistsRepository extends Repository<Playlist> {
    async getAvailablePlaylists(userId: string) {
        return await this.createQueryBuilder("playlist")
            .where("not exists(" +
                "select * from PlaylistUser where PlaylistsId = playlist.Id and UsersId = :userId)", {userId})
            .getMany();
    }

    async getLikedPlaylists(userId: string) {
        return await this.createQueryBuilder("playlist")
            .where("playlist.Id in (" +
                "select PlaylistsId from PlaylistUser where UsersId = :userId)", {userId})
            .getMany()
    }
}
