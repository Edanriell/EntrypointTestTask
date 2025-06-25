import { httpClient } from "@shared/lib/http-client";
import { AccessTokenResponse, LoginUserRequest } from "../model";

export const loginUser = async (credentials: LoginUserRequest): Promise<AccessTokenResponse> => {
	return httpClient.post("/users/login", credentials);
};
