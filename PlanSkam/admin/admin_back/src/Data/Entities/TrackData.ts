import {Entity, Column, PrimaryGeneratedColumn} from 'typeorm';

@Entity('TrackDatas')
export class TrackData {
    @PrimaryGeneratedColumn()
    Id: number;
}