import {BadRequestException, Controller, Get, Post, Query, UseGuards} from "@nestjs/common";
import {AuthorsRepository} from "../Data/Repositories/Authors.repository";
import {JwtAuthGuard} from "../auth.guard";

@Controller('authors')
@UseGuards(JwtAuthGuard)
export class AuthorsController {
    constructor(private readonly authorsRepo: AuthorsRepository) {
    }

    @Get('search')
    async search(@Query('query') query: string) {
        return this.authorsRepo.search(query);
    }

    @Get('getWithTracks')
    async getWithTracks(@Query('authorId') authorId: number) {
        return this.authorsRepo.getWithTracks(authorId);
    }
    
    @Post('changeName')
    async changeName(@Query('id') id: number, @Query('name') name: string){
        if(await this.authorsRepo.changeName(id, name))
            return `name changed to ${name}`;
        throw new BadRequestException();
    }
}
