import { Trim } from 'class-sanitizer';
import { IsEmail, IsString } from 'class-validator'

export class LoginDto{
    @Trim()
    @IsEmail()
    public readonly userName: string;
    
    @IsString()
    public readonly password: string;
}