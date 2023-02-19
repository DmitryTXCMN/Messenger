import {EntityRepository, Repository} from "typeorm";
import {Role} from "../Entities/Role";

@EntityRepository(Role)
export class RolesRepository extends Repository<Role>{

}
