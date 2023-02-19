import {Module} from "@nestjs/common";
import {TypeOrmModule} from "@nestjs/typeorm";
import {Playlist} from "../Data/Entities/Playlist";
import {PlaylistsRepository} from "../Data/Repositories/Playlists.repository";
import {PlaylistsController} from "../Controllers/Playlists.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Playlist, PlaylistsRepository])],
    controllers: [PlaylistsController]
})
export class PlaylistsModule {
}