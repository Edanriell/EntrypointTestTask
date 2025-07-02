export interface SessionInfo {
	isValid: boolean;
	expiresAt: number;
	user: {
		id: string;
		email: string;
		roles: string[];
		permissions: string[];
	};
}
