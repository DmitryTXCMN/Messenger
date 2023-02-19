import {Controller, Get, Post, Query, BadRequestException, UseGuards} from "@nestjs/common";
import {UsersRepository} from "../Data/Repositories/Users.repository";
import {JwtAuthGuard} from "../auth.guard";

@Controller('users')
@UseGuards(JwtAuthGuard)
export class UsersController {
    constructor(private readonly usersRepo: UsersRepository) {
    }

    @Get('getAll')
    async getAll() {
        return this.usersRepo.GetAll();
    }

    @Get('')
    async index(@Query('id') id: string) {
        return await this.usersRepo.GetUser(id);
    }

    @Get('isAuthor')
    async isAuthor(@Query('id') id: string) {
        return await this.usersRepo.IsAuthor(id);
    }

    @Post('makeAuthor')
    async makeAuthor(@Query('id') id: string) {
        if (await this.usersRepo.MakeAuthor(id))
            return "User is now author";
        throw new BadRequestException();
    }

    @Post('makeNotAuthor')
    async makeNotAuthor(@Query('id') id: string) {
        if (await this.usersRepo.MakeNotAuthor(id))
            return "User is not author now";
        throw new BadRequestException();
    }

    @Get('getFavTracks')
    async getFavTracks(@Query('id') id: string) {
        return await this.usersRepo.getFavTracks(id);
    }
    
    @Post('addTrackToFavourites')
    async addTrackToFavourites(@Query('userId') userId: string, @Query('trackId') trackId: number){
        if(await this.usersRepo.addTrackToFavourites(userId, trackId))
            return "Track liked";
        throw new BadRequestException();
    }
    
    @Post('removeTrackFromFavourites')
    async removeTrackFromFavourites(@Query('userId') userId: string, @Query('trackId') trackId: number){
        if(await this.usersRepo.removeTrackFromFavourites(userId, trackId))
            return "Track unliked";
        throw new BadRequestException();
    }

    @Post('addPlaylistToLiked')
    async addPlaylistToLiked(@Query('userId') userId: string, @Query('playlistId') playlistId: number) {
        if (await this.usersRepo.addPlaylistToLiked(userId, playlistId))
            return "Playlist added"
        throw new BadRequestException();
    }

    @Post('removePlaylistFromLiked')
    async removePlaylistFromLiked(@Query('userId') userId: string, @Query('playlistId') playlistId: number) {
        if (await this.usersRepo.removePlaylistFromLiked(userId, playlistId))
            return "Playlist removed"
        throw new BadRequestException();
    }

    @Post('changeEmail')
    async changeName(@Query('id') id: string, @Query('email') email: string) {
        if (await this.usersRepo.changeEmail(id, email))
            return "Email changed to " + email
        throw new BadRequestException();
    }
}