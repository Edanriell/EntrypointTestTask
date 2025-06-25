import { httpClient } from "@shared/lib/http-client";
import { UpdateUserRequest } from "../model";

export const updateUser = async (userData: UpdateUserRequest): Promise<void> => {
	return httpClient.put("/users/update", userData);
};
