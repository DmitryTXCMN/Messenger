import {Injectable} from "@nestjs/common";
import {PassportStrategy} from '@nestjs/passport'
import {Strategy, ExtractJwt} from "passport-jwt";
import {UsersRepository} from "./Data/Repositories/Users.repository";

@Injectable()
export class JwtStrategy extends PassportStrategy(Strategy) {
    constructor(private readonly usersRepo: UsersRepository) {
        super({
            jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
            secretOrKey: 'iuerwpfnoas',
            ignoreExpiration: true
        });
    }

    private async validate(payload: any) {
        return await this.usersRepo.findOne(payload.id);
    }
}