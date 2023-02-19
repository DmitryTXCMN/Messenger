import {Module} from "@nestjs/common";
import {PassportModule} from '@nestjs/passport';
import {JwtModule} from '@nestjs/jwt'
import {TypeOrmModule} from "@nestjs/typeorm";
import {UsersRepository} from "../Data/Repositories/Users.repository";
import {User} from "../Data/Entities/User";
import {AuthController} from "../Controllers/auth.controller";
import {JwtStrategy} from "../auth.strategy";

@Module({
    imports: [
        PassportModule.register({defaultStrategy: 'jwt', property: 'user'}),
        JwtModule.registerAsync({
            useFactory: () => ({
                secret: 'iuerwpfnoas',
                signOptions: {expiresIn: '100d'}
            })
        }),
        TypeOrmModule.forFeature([User, UsersRepository])
    ],
    controllers: [AuthController],
    providers: [JwtStrategy]
})
export class AuthModule {
}