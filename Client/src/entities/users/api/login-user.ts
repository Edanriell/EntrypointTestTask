import { httpClient } from "@shared/api/api-client";
import { AccessTokenResponse, LoginUserRequest } from "../model";

export const loginUser = async (credentials: LoginUserRequest): Promise<AccessTokenResponse> => {
	return httpClient.post("/users/login", credentials);
};
