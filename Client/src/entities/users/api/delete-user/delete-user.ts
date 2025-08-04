import { apiClient } from "@shared/api";

import type { DeleteUserCommand } from "./delete-user-command";

export const deleteUser = async ({ userId }: DeleteUserCommand): Promise<void> => {
	return apiClient.delete<void>(`/users/delete/${userId}`);
};
