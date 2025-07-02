import { apiClient } from "@shared/api";

export const deleteUser = async (userId: string): Promise<void> => {
	return apiClient.delete<void>(`/users/delete/${userId}`);
};
