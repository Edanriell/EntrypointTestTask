export type AccessTokenResponse = {
	accessToken: string;
	expiresIn: number;
	refreshToken: string;
};

export type AuthProvider = {
	login: (options?: any) => Promise<void>;
	logout: () => Promise<void>;
	refreshSession: () => Promise<any>;
};

export type AuthSession = {
	user: User;
	error?: string;
	isAuthenticated: boolean;
	isLoading: boolean;
};

export type AuthStrategy = "keycloak" | "credentials";

export type AuthUser = {
	id: string;
	email: string;
	name: string;
	image?: string;
	accessToken?: string;
	refreshToken?: string;
	roles?: string[];
	permissions?: string[];
};

export type LoginUserRequest = {
	email: string;
	password: string;
};

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

export type LogoutRequest = {
	refreshToken?: string;
	logoutFromAllDevices?: boolean;
};

export type SessionInfo = {
	isValid: boolean;
	expiresAt: number;
	user: {
		id: string;
		email: string;
		roles: string[];
		permissions: string[];
	};
};

export type User = {
	id: string;
	name?: string;
	email?: string;
	image?: string;
	roles: string[];
	permissions: string[];
};
