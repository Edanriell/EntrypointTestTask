export type LoginUserResponse = {
	accessToken: string;
	refreshToken?: string;
	expiresIn: number;
	tokenType?: string;
	user?: {
		id: string;
		email: string;
		name: string;
		roles?: string[];
		permissions?: string[];
	};
};
