import {Entity, Column, PrimaryColumn, ManyToMany, JoinTable, OneToOne, JoinColumn} from 'typeorm';
import {Role} from "./Role";
import {Playlist} from "./Playlist";

//некоторые столбцы из бд намеренно не перенесены, при необходимости добавить
@Entity('AspNetUsers')
export class User {
    @PrimaryColumn({type: "nvarchar"})
    Id: string;

    @Column()
    UserName: string;
    
    @Column()
    NormalizedUserName: string;

    @ManyToMany(() => Playlist, playlist => playlist.Users)
    @JoinTable({
        name: "PlaylistUser",
        joinColumn: {
            name: "UsersId",
            referencedColumnName: "Id"
        },
        inverseJoinColumn: {
            name: "PlaylistsId",
            referencedColumnName: "Id"
        }
    })
    Playlists: Playlist[]

    @ManyToMany(() => Role, role => role.Users)
    @JoinTable({
        name: "AspNetUserRoles",
        joinColumn: {
            name: "UserId",
            referencedColumnName: "Id"
        },
        inverseJoinColumn: {
            name: "RoleId",
            referencedColumnName: "Id"
        }
    })
    Roles: Role[]

    @Column()
    Email: string;
    
    @Column()
    NormalizedEmail: string;
    
    @Column()
    FavouriteTracksId: number;
    
    @Column()
    PasswordHash: string;
    
    @Column()
    EmailConfirmed: boolean;
    
    @Column()
    PhoneNumberConfirmed: boolean;
    
    @Column()
    TwoFactorEnabled: boolean;
    
    @Column()
    LockoutEnabled: boolean;
    
    @Column()
    AccessFailedCount: number;
    
    @Column()
    OwnedPlaylistsId: number;
}