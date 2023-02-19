import {Entity, Column, PrimaryGeneratedColumn, TableForeignKey, ManyToMany, JoinTable} from 'typeorm';
import {User} from "./User";

@Entity('Playlists')
export class Playlist {
    @PrimaryGeneratedColumn()
    Id: number;

    @Column()
    Name: string;

    @ManyToMany(() => User, user => user.Playlists)
    @JoinTable()
    Users: User[]
}