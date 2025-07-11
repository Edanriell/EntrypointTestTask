import { apiClient } from "@shared/api";
import { AccessTokenResponse } from "@features/authentication/general";

export const refreshToken = async (refreshToken: string): Promise<AccessTokenResponse> => {
	// Not implemented on the back-end, just an example
	return apiClient.post<AccessTokenResponse>("/users/refresh", { refreshToken });
};
