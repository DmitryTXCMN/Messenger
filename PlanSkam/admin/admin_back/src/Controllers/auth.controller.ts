import {Controller, Post, Body, HttpException, HttpStatus, UseGuards} from "@nestjs/common";
import {UsersRepository} from "../Data/Repositories/Users.repository";
import {LoginDto} from "../Dto/login.dto";
import * as bcryptjs from 'bcryptjs';
import {JwtService} from '@nestjs/jwt'
import {RegisterDto} from "../Dto/register.dto";
import {randomUUID} from 'crypto';
import {User} from "../Data/Entities/User";
import {JwtAuthGuard} from "../auth.guard";

@Controller('auth')
export class AuthController {
    constructor(private readonly usersRepo: UsersRepository,
                private readonly jwtService: JwtService) {
    }

    @Post('login')
    async login(@Body() loginDto: LoginDto) {
        console.log(loginDto)
        const user = await this.usersRepo.findOne({where: {UserName: loginDto.userName}})
        if (user == null) {
            throw new HttpException('wrong username', HttpStatus.BAD_REQUEST);
        }
        if (!bcryptjs.compareSync(loginDto.password, user.PasswordHash)) {
            throw new HttpException('wrong pass, the right one is AIF29fsjd', HttpStatus.BAD_REQUEST);
        }
        return {
            token: this.jwtService.sign({id: user.Id, email: user.Email}),
            user: {
                userName: user.UserName,
                email: user.Email
            }
        };
    }

    @Post('register')
    @UseGuards(JwtAuthGuard)
    async register(@Body() registerDto: RegisterDto) {
        console.log(registerDto);
        
        const normalizedUserName = registerDto.userName.toUpperCase();
        if (await this.usersRepo.findOne({where: {NormalizedUserName: normalizedUserName}}) != null) {
            throw new HttpException(`username ${registerDto.userName} already exists`, HttpStatus.BAD_REQUEST);
        }
        
        const normalizedEmail = registerDto.email.toUpperCase();
        if (await this.usersRepo.findOne({where: {NormalizedEmail: normalizedEmail}}) != null) {
            throw new HttpException(`email ${registerDto.email} already used`, HttpStatus.BAD_REQUEST);
        }
        
        const user = new User();
        user.Id = randomUUID();
        user.UserName = registerDto.userName;
        user.NormalizedUserName = normalizedUserName;
        user.Email = registerDto.email;
        user.NormalizedEmail = normalizedEmail;
        user.PasswordHash = bcryptjs.hashSync(registerDto.password, bcryptjs.genSaltSync(10));
        user.EmailConfirmed = true;
        user.PhoneNumberConfirmed = true;
        user.TwoFactorEnabled = false;
        user.LockoutEnabled = true;
        user.AccessFailedCount = 0;
        user.FavouriteTracksId = 0;
        user.OwnedPlaylistsId = 0;
        
        return await this.usersRepo.save(user);
    }
}