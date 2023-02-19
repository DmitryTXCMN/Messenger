import {Entity, Column, PrimaryGeneratedColumn, ManyToOne, JoinColumn} from 'typeorm';
import {Author} from "./Author";

@Entity('Tracks')
export class Track{
    @PrimaryGeneratedColumn()
    Id: number;
    
    @Column()
    Name: string;
    
    @ManyToOne(() => Track)
    @JoinColumn({
        name: "AuthorId"
    })
    Author: Author;
    
    @Column()
    TrackDataId: number;
}