import { httpClient } from "@shared/lib/http-client";
import { RegisterUserRequest } from "../model";

export const createUser = async (userData: RegisterUserRequest): Promise<string> => {
	return httpClient.post("/users/register", userData);
};