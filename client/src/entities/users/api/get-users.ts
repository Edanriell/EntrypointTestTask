import { httpClient } from "@shared/lib/http-client";
import { User, UserListQuery, UserResponse } from "../model";

// Get all users (to be implemented in backend)
export const getUsers = async (params?: UserListQuery): Promise<User[]> => {
	return httpClient.get("/users/all", { params });
};

// Get user by ID (to be implemented in backend)
export const getUserById = async (userId: string): Promise<User> => {
	return httpClient.get(`/users/${userId}`);
};

// Get logged in user
export const getLoggedInUser = async (): Promise<UserResponse> => {
	return httpClient.get("/users/me");
};

// Get clients
export const getClients = async (): Promise<UserResponse[]> => {
	return httpClient.get("/users");
};
