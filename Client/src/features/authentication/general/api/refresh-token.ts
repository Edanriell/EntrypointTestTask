import { AccessTokenResponse } from "@features/authentication/general";

import { apiClient } from "@shared/api";

export const refreshToken = async (refreshToken: string): Promise<AccessTokenResponse> => {
	// Not implemented on the back-end, just an example
	return apiClient.post<AccessTokenResponse>("/users/refresh", { refreshToken });
};
