import { z } from "zod";

export enum Gender {
	Male = 0,
	Female = 1
}

export const GenderEnum = z.nativeEnum(Gender);
