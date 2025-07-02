import { apiClient } from "@shared/api";
import type { RegisterUserRequest } from "../model";

export const createUser = async (data: RegisterUserRequest): Promise<string> => {
	return apiClient.post<string>("/users/register", data);
};
