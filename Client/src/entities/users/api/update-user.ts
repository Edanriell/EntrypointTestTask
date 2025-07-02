import { apiClient } from "@shared/api";
import type { UpdateUserRequest } from "../model";

export const updateUser = async (data: UpdateUserRequest): Promise<void> => {
	return apiClient.put<void>("/users/update", data);
};
