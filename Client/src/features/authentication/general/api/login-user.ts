import { apiClient } from "@shared/api";

import { AccessTokenResponse, LoginUserRequest } from "@features/authentication/general/model";

export const loginUser = async (credentials: LoginUserRequest): Promise<AccessTokenResponse> => {
	return apiClient.post<AccessTokenResponse>("/users/login", credentials);
};
