export type AuthProvider = {
	login: (options?: any) => Promise<void>;
	logout: () => Promise<void>;
	refreshSession: () => Promise<any>;
};
