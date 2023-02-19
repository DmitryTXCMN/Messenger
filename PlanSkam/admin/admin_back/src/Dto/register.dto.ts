import { Trim } from 'class-sanitizer';
import { IsEmail, IsString, Length } from 'class-validator'

export class RegisterDto{
    @Trim()
    @IsEmail()
    public readonly userName: string;

    @IsString()
    @Length(6)
    public readonly password: string;
    
    @IsEmail()
    public readonly email: string;
}