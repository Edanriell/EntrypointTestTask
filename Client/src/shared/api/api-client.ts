import axios, { AxiosError, AxiosInstance, AxiosRequestConfig } from "axios";
import { getSession } from "next-auth/react";

// TODO FSD CONFLICT HERE
import { TokenManager } from "@features/authentication/general";

import { API_CONFIG } from "@shared/config";

export class ApiClient {
	// private instance: AxiosInstance;
	// private readonly baseUrl: string;
	//
	// constructor(baseUrl?: string) {
	// 	this.baseUrl = baseUrl || `${API_CONFIG.BASE_URL}/${API_CONFIG.VERSION}`;
	//
	// 	this.instance = axios.create({
	// 		baseURL: this.baseUrl,
	// 		timeout: 10000,
	// 		headers: {
	// 			"Content-Type": "application/json"
	// 		}
	// 	});
	//
	// 	this.setupInterceptors();
	// }
	private instance: AxiosInstance;
	private readonly baseUrl: string;

	constructor(baseUrl?: string) {
		this.baseUrl = baseUrl || `${API_CONFIG.BASE_URL}/${API_CONFIG.VERSION}`;

		this.instance = axios.create({
			baseURL: this.baseUrl,
			timeout: 10000,
			headers: {
				"Content-Type": "application/json"
			}
		});

		this.setupInterceptors();
	}

	public async get<TResult = unknown>(
		endpoint: string,
		queryParams?: Record<string, string | number>,
		config?: AxiosRequestConfig
	): Promise<TResult> {
		try {
			const params = queryParams ? { params: queryParams } : {};
			const response = await this.instance.get(endpoint, { ...params, ...config });
			return this.handleResponse<TResult>(response);
		} catch (error) {
			return this.handleError(error as AxiosError);
		}
	}

	public async post<TResult = unknown, TData = any>(
		endpoint: string,
		data?: TData,
		config?: AxiosRequestConfig
	): Promise<TResult> {
		try {
			const response = await this.instance.post(endpoint, data, config);
			return this.handleResponse<TResult>(response);
		} catch (error) {
			return this.handleError(error as AxiosError);
		}
	}

	public async put<TResult = unknown, TData = any>(
		endpoint: string,
		data?: TData,
		config?: AxiosRequestConfig
	): Promise<TResult> {
		try {
			const response = await this.instance.put(endpoint, data, config);
			return this.handleResponse<TResult>(response);
		} catch (error) {
			return this.handleError(error as AxiosError);
		}
	}

	public async patch<TResult = unknown, TData = any>(
		endpoint: string,
		data?: TData,
		config?: AxiosRequestConfig
	): Promise<TResult> {
		try {
			const response = await this.instance.patch(endpoint, data, config);
			return this.handleResponse<TResult>(response);
		} catch (error) {
			return this.handleError(error as AxiosError);
		}
	}

	public async delete<TResult = unknown>(
		endpoint: string,
		config?: AxiosRequestConfig
	): Promise<TResult> {
		try {
			const response = await this.instance.delete(endpoint, config);
			return this.handleResponse<TResult>(response);
		} catch (error) {
			return this.handleError(error as AxiosError);
		}
	}

	private async handleResponse<TResult>(response: any): Promise<TResult> {
		return response.data;
	}

	// private async handleError(error: AxiosError): Promise<never> {
	// 	if (error.response?.status === 401) {
	// 		// Token might be expired, try to refresh
	// 		const session = await getSession();
	// 		if (session?.error === "RefreshAccessTokenError") {
	// 			// Redirect to login
	// 			window.location.href = "/sign-in";
	// 		}
	// 	}
	//
	// 	throw error;
	// }

	private async handleError(error: AxiosError): Promise<never> {
		if (error.response?.status === 401) {
			// Check if this is a login request - don't redirect for login failures
			const isLoginRequest = error.config?.url?.includes("/users/login");

			if (!isLoginRequest) {
				// Only redirect for non-login 401s (token expiration)
				const session = await getSession();
				if (session?.error === "RefreshAccessTokenError") {
					window.location.href = "/sign-in";
				}
			}
		}

		// Always throw the error so the mutation can handle it
		throw error;
	}

	// private setupInterceptors() {
	// 	// Request interceptor to add auth token
	// 	this.instance.interceptors.request.use(
	// 		async (config) => {
	// 			const session = await getSession();
	// 			if (session?.accessToken) {
	// 				config.headers.Authorization = `Bearer ${session.accessToken}`;
	// 			}
	// 			return config;
	// 		},
	// 		(error) => Promise.reject(error)
	// 	);
	//
	// 	// Response interceptor for global error handling
	// 	this.instance.interceptors.response.use(
	// 		(response) => response,
	// 		(error) => Promise.reject(error)
	// 	);
	// }
	private setupInterceptors() {
		// Request interceptor with optimized token handling
		this.instance.interceptors.request.use(
			async (config) => {
				// Try to get a token from server-side if available
				if (typeof window === "undefined") {
					// Server-side: get a fresh token
					try {
						const token = await TokenManager.getAccessToken();
						if (token) {
							config.headers.Authorization = `Bearer ${token}`;
						}
					} catch (error) {
						console.error("Error getting server-side token:", error);
					}
				} else {
					// Client-side: use session
					const session = await getSession();
					if (session?.user) {
						// For client-side requests, we'll need to get token differently
						// We might want to create an API route to get fresh tokens
						console.warn("Client-side requests may need fresh token handling");
					}
				}
				return config;
			},
			(error) => Promise.reject(error)
		);

		// Response interceptor for global error handling
		// this.instance.interceptors.response.use(
		// 	(response) => response,
		// 	async (error) => {
		// 		if (error.response?.status === 401) {
		// 			// Handle token expiration
		// 			if (typeof window !== "undefined") {
		// 				window.location.href = "/sign-in";
		// 			}
		// 		}
		// 		return Promise.reject(error);
		// 	}
		// );

		this.instance.interceptors.response.use(
			(response) => response,
			async (error) => {
				if (error.response?.status === 401) {
					// Don't redirect for login requests
					const isLoginRequest = error.config?.url?.includes("/users/login");

					if (!isLoginRequest && typeof window !== "undefined") {
						window.location.href = "/sign-in";
					}
				}
				return Promise.reject(error);
			}
		);
	}
}

// Export a default instance
export const apiClient = new ApiClient();

// Keep backward compatibility
// export const httpClient = apiClient;
