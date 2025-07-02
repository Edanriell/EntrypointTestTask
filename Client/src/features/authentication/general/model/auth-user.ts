export interface AuthUser {
	id: string;
	email: string;
	name: string;
	image?: string;
	accessToken?: string;
	refreshToken?: string;
	roles?: string[];
	permissions?: string[];
}

// TODO
// Probabbly delete
