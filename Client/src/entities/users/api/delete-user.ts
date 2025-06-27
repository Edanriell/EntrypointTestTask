import { httpClient } from "@shared/api/api-client";

export const deleteUser = async (userId: string): Promise<void> => {
	return httpClient.delete(`/users/delete/${userId}`);
};
