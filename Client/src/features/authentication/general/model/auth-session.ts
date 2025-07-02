import type { User } from "@features/authentication/general/model";

export type AuthSession = {
	user: User;
	error?: string;
	isAuthenticated: boolean;
	isLoading: boolean;
};
