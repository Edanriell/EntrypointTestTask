import { httpClient } from "@shared/lib/http-client";

export const deleteUser = async (userId: string): Promise<void> => {
	return httpClient.delete(`/users/delete/${userId}`);
};
