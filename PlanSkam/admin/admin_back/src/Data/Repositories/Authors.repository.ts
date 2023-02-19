import {EntityRepository, Repository} from "typeorm";
import {Author} from "../Entities/Author";

@EntityRepository(Author)
export class AuthorsRepository extends Repository<Author> {
    async getWithTracks(authorId: number) {
        return await this.findOne(authorId, {relations: ["Tracks"]});
    }
    
    async search(query: string){
        return await this.createQueryBuilder("author")
            .where("author.Name like :q", {q: `%${query}%`})
            .getMany();
    }
    
    async changeName(id: number, name: string){
        const author = await this.findOne(id);
        if(author == null)
            return false;
        author.Name = name;
        await this.save(author);
        return true;
    }
}
