import axios, { AxiosInstance, AxiosRequestConfig } from "axios";
import { getSession } from "next-auth/react";

import { API_CONFIG } from "@shared/config/api";

class HttpClient {
	private instance: AxiosInstance;

	constructor() {
		this.instance = axios.create({
			baseURL: `${API_CONFIG.BASE_URL}/${API_CONFIG.VERSION}`,
			timeout: 10000,
			headers: {
				"Content-Type": "application/json"
			}
		});

		this.setupInterceptors();
	}

	async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
		const response = await this.instance.get(url, config);
		return response.data;
	}

	async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
		const response = await this.instance.post(url, data, config);
		return response.data;
	}

	async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
		const response = await this.instance.put(url, data, config);
		return response.data;
	}

	async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
		const response = await this.instance.delete(url, config);
		return response.data;
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

		// Response interceptor for error handling
		this.instance.interceptors.response.use(
			(response) => response,
			async (error) => {
				if (error.response?.status === 401) {
					// Token might be expired, try to refresh
					const session = await getSession();
					if (session?.error === "RefreshAccessTokenError") {
						// Redirect to login
						// TODO
						// FIX URL
						window.location.href = "/auth/signin";
					}
				}
				return Promise.reject(error);
			}
		);
	}
}

export const httpClient = new HttpClient();
