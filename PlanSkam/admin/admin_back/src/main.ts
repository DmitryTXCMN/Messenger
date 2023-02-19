import { NestFactory } from '@nestjs/core';
import { AppModule } from './Modules/app.module';
import {NestExpressApplication} from "@nestjs/platform-express";

async function bootstrap() {
  const app : NestExpressApplication = await NestFactory.create(AppModule);
  app.enableCors({
    allowedHeaders: "*"
  });
  await app.listen(3000);
}
bootstrap();
