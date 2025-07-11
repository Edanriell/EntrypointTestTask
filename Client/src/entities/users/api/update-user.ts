import { apiClient } from "@shared/api";

import type { UpdateUserRequest } from "../model";

export const updateUser = async (userId: string, userData: UpdateUserRequest): Promise<void> => {
	return apiClient.put<void>(`/users/update/${userId}`, userData);
};
