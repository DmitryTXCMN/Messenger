import {EntityRepository, Repository} from "typeorm";
import {TrackData} from "../Entities/TrackData";

@EntityRepository(TrackData)
export class TrackDatasRepository extends Repository<TrackData>{

}
