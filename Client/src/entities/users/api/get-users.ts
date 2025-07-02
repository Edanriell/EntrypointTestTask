import { apiClient } from "@shared/api";
import type { User, UserListQuery, UserResponse } from "../model";

export const getUsers = async (query?: UserListQuery): Promise<User[]> => {
	const params = new URLSearchParams();

	// TODO
	// Implement cursor pagination ?
	// if (query?.page) params.append("page", query.page.toString());
	// if (query?.limit) params.append("limit", query.limit.toString());
	// if (query?.search) params.append("search", query.search);

	const queryString = params.toString();
	const url = queryString ? `/users?${queryString}` : "/users";

	return apiClient.get<User[]>(url);
};

export const getUserById = async (id: string): Promise<User> => {
	return apiClient.get<User>(`/users/${id}`);
};

export const getLoggedInUser = async (): Promise<UserResponse> => {
	return apiClient.get<UserResponse>("/users/me");
};

export const getClients = async (): Promise<User[]> => {
	return apiClient.get<User[]>("/users");
};
