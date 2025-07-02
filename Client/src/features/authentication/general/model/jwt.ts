import { DefaultJWT } from "next-auth/jwt";

declare module "next-auth/jwt" {
	interface JWT extends DefaultJWT {
		accessToken?: string;
		refreshToken?: string;
		expiresAt?: number;
		idToken?: string;
		error?: string;
		roles?: string[];
		permissions?: string[];
	}
}
