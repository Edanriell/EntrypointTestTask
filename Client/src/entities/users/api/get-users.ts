import { apiClient } from "@shared/api";

import type { UserResponse } from "../model";

export const getLoggedInUser = async (): Promise<UserResponse> => {
	return apiClient.get<UserResponse>("/users/me");
};
