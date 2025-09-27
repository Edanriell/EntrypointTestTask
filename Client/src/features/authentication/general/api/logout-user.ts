import { LogoutRequest } from "@features/authentication/general";

import { apiClient } from "@shared/api";

export const logoutUser = async (request?: LogoutRequest): Promise<void> => {
	// Not implemented on the back-end, just an example
	return apiClient.post<void>("/users/logout", request);
};
