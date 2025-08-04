import { apiClient } from "@shared/api";

import { GetLoggedInUserResponse } from "./get-logged-in-user-response";

export const getLoggedInUser = async (): Promise<GetLoggedInUserResponse> => {
	return apiClient.get<GetLoggedInUserResponse>("/users/me");
};
