import {Controller, Get, Post, Query, UseGuards} from "@nestjs/common";
import {PlaylistsRepository} from "../Data/Repositories/Playlists.repository";
import {JwtAuthGuard} from "../auth.guard";

@Controller('playlists')
@UseGuards(JwtAuthGuard)
export class PlaylistsController {
    constructor(private readonly playlistsRepository: PlaylistsRepository) {
    }

    @Get('getAvailablePlaylists')
    async getAvailablePlaylists(@Query('userId') userId: string) {
        return await this.playlistsRepository.getAvailablePlaylists(userId);
    }
    
    @Get('getLikedPlaylists')
    async getLikedPlaylists(@Query('userId') userId: string) {
        return await this.playlistsRepository.getLikedPlaylists(userId);
    }
    
    
}