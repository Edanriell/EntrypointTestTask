import axios, { AxiosError, AxiosInstance, AxiosRequestConfig } from "axios";
import { getSession } from "next-auth/react";

import { API_CONFIG } from "@shared/config/api";

export class ApiClient {
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

	private async handleError(error: AxiosError): Promise<never> {
		if (error.response?.status === 401) {
			// Token might be expired, try to refresh
			const session = await getSession();
			if (session?.error === "RefreshAccessTokenError") {
				// Redirect to login
				window.location.href = "/sign-in";
			}
		}

		// You can add more error handling logic here
		const errorMessage =
			(error.response?.data as any)?.message || error.message || "An error occurred";
		throw new Error(errorMessage);
	}

	private setupInterceptors() {
		// Request interceptor to add auth token
		this.instance.interceptors.request.use(
			async (config) => {
				const session = await getSession();
				if (session?.accessToken) {
					config.headers.Authorization = `Bearer ${session.accessToken}`;
				}
				return config;
			},
			(error) => Promise.reject(error)
		);

		// Response interceptor for global error handling
		this.instance.interceptors.response.use(
			(response) => response,
			(error) => Promise.reject(error)
		);
	}
}

// Export a default instance
export const apiClient = new ApiClient();

// Keep backward compatibility
export const httpClient = apiClient;
