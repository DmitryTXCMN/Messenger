import {Module} from "@nestjs/common";
import {TypeOrmModule} from "@nestjs/typeorm";
import {User} from "../Data/Entities/User";
import {UsersRepository} from "../Data/Repositories/Users.repository";
import {UsersController} from "../Controllers/Users.controller";

@Module({
    imports: [TypeOrmModule.forFeature([User, UsersRepository])],
    controllers: [UsersController]
})
export class UsersModule {
}