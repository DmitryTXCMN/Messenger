import {AuthGuard, IAuthGuard} from '@nestjs/passport'
import {ExecutionContext, Injectable} from '@nestjs/common';
import {User} from "./Data/Entities/User";
import {Request} from 'express';

@Injectable()
export class JwtAuthGuard extends AuthGuard('jwt') implements IAuthGuard {
    public handleRequest(err: unknown, user: User): any {
        return user;
    }

    public async canActivate(context: ExecutionContext): Promise<boolean> {
        await super.canActivate(context);
        const request = context.switchToHttp().getRequest();
        return !!request.user;
    }
}