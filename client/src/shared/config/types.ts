import { DefaultSession, DefaultUser } from "next-auth";
import { DefaultJWT } from "next-auth/jwt";

declare module "next-auth" {
	interface Session extends DefaultSession {
		accessToken?: string;
		refreshToken?: string;
		error?: string;
	}

	interface User extends DefaultUser {
		accessToken?: string;
		refreshToken?: string;
	}
}

declare module "next-auth/jwt" {
	interface JWT extends DefaultJWT {
		accessToken?: string;
		refreshToken?: string;
		expiresAt?: number;
		idToken?: string;
		error?: string;
	}
}

export interface AuthUser {
	id: string;
	email: string;
	name: string;
	image?: string;
	accessToken?: string;
	refreshToken?: string;
}
