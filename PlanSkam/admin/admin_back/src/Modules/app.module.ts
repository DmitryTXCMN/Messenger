import {Module} from '@nestjs/common';
import {TypeOrmModule} from '@nestjs/typeorm'
import {Track} from "../Data/Entities/Track";
import {UsersModule} from "./Users.module";
import {User} from "../Data/Entities/User";
import {Role} from "../Data/Entities/Role";
import {Playlist} from "../Data/Entities/Playlist";
import {PlaylistsModule} from "./Playlists.module";
import {Author} from "../Data/Entities/Author";
import {AuthorsModule} from "./Authors.module";
import {TracksModule} from "./Tracks.module";
import {TrackData} from "../Data/Entities/TrackData";
import {AuthModule} from "./auth.module";

@Module({
    imports: [
        TypeOrmModule.forRoot({
            type: "mssql",
            host: "planscam.mssql.somee.com",
            database: "planscam",
            port: 1433,
            username: "erererererer123_SQLLogin_1",
            password: "2qnmximctf",
            entities: [Track, Role, User, Playlist, Author, TrackData]
        }),
        UsersModule,
        PlaylistsModule,
        AuthorsModule,
        TracksModule,
        AuthModule
    ]
})
export class AppModule {
}
