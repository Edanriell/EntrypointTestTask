import { DefaultSession, DefaultUser } from "next-auth";

declare module "next-auth" {
	interface Session extends DefaultSession {
		accessToken?: string;
		refreshToken?: string;
		error?: string;
		user: {
			id: string;
			email: string;
			name: string;
			image?: string;
			roles?: string[];
			permissions?: string[];
		} & DefaultSession["user"];
	}

	interface User extends DefaultUser {
		accessToken?: string;
		refreshToken?: string;
		roles?: string[];
		permissions?: string[];
	}
}