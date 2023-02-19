import {Module} from "@nestjs/common";
import {TypeOrmModule} from "@nestjs/typeorm";
import {Author} from "../Data/Entities/Author";
import {AuthorsRepository} from "../Data/Repositories/Authors.repository";
import {AuthorsController} from "../Controllers/Authors.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Author, AuthorsRepository])],
    controllers: [AuthorsController]
})
export class AuthorsModule {
}