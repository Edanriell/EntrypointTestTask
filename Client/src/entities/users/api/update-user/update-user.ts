import { apiClient } from "@shared/api";

import type { UpdateUserCommand } from "./update-user-command";

export const updateUser = async ({ userId, updatedUserData }: UpdateUserCommand): Promise<void> => {
	return apiClient.put<void>(`/users/update/${userId}`, updatedUserData);
};
