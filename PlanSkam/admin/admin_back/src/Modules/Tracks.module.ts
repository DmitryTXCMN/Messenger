import {Module} from "@nestjs/common";
import {TypeOrmModule} from "@nestjs/typeorm";
import {Track} from "../Data/Entities/Track";
import {TracksRepository} from "../Data/Repositories/Tracks.repository";
import {TracksController} from "../Controllers/Tracks.controller";
import {TrackData} from "../Data/Entities/TrackData";
import {TrackDatasRepository} from "../Data/Repositories/TrackDatas.repository";

@Module({
    imports: [
        TypeOrmModule.forFeature([Track, TracksRepository]),
        TypeOrmModule.forFeature([TrackData, TrackDatasRepository])
    ],
    controllers: [TracksController]
})
export class TracksModule {
}