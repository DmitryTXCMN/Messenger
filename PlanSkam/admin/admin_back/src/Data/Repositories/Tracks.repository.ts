import {EntityRepository, Repository, getRepository} from "typeorm";
import {Track} from "../Entities/Track";
import {TrackData} from "../Entities/TrackData";

@EntityRepository(Track)
export class TracksRepository extends Repository<Track> {
    async removeTrack(id: number) {
        const track = await this.findOne(id);
        if (track == null)
            return false;
        await this.remove(track);
        const trackDatasRepo = getRepository(TrackData);
        const trackData = await trackDatasRepo.findOne(track.TrackDataId);
        if (trackData != null)
            await trackDatasRepo.remove(trackData);
        return true;
    }

    async searchTracks(query: string): Promise<Track[]> {
        const t = await this.createQueryBuilder("track")
            .where("track.Name like :q", {q: `%${query}%`})
            .getMany()
        console.log(t);
        return t;
    }

    async changeName(trackId: number, name: string): Promise<boolean> {
        const track = await this.findOne(trackId);
        if(track == null)
            return false;
        track.Name = name;
        await this.save(track);
        return true;
    }
}
