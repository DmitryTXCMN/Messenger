import {Entity, Column, PrimaryGeneratedColumn, OneToMany, JoinColumn} from 'typeorm';
import {Track} from "./Track";

@Entity('Authors')
export class Author {
    @PrimaryGeneratedColumn()
    Id: number;

    @Column()
    Name: string;

    @Column()
    PictureId: number;

    @Column()
    UserId: string;
    
    @OneToMany(() => Track, track => track.Author)
    @JoinColumn({
        referencedColumnName: "AuthorId"
    })
    Tracks : Track[];
}