import { apiClient } from "@shared/api";

export const getCurrentSession = async (): Promise<any> => {
	return apiClient.get("/auth/session");
};
