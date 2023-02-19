import {Column, Entity, JoinTable, ManyToMany, PrimaryColumn} from "typeorm";
import {User} from "./User";

@Entity('AspNetRoles')
export class Role {
    @PrimaryColumn({type: "nvarchar"})
    Id: string;

    @Column()
    Name: string;

    @ManyToMany(() => User, (user) => user.Roles)
    @JoinTable({
        name: "AspNetUserRoles",
        joinColumn: {
            name: "RoleId",
            referencedColumnName: "Id"
        },
        inverseJoinColumn: {
            name: "UserId",
            referencedColumnName: "Id"
        }
    })
    Users: User[]
}