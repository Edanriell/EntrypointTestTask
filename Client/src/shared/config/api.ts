// TODO EXTEND

export const API_CONFIG = {
	BASE_URL: process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:5000/api",
	VERSION: process.env.NEXT_PUBLIC_API_VERSION || "v1"
} as const;

// TODO
// USE IN http-client
export const getApiUrl = (endpoint: string) =>
	`${API_CONFIG.BASE_URL}/${API_CONFIG.VERSION}${endpoint}`;
