import { apiClient } from "@shared/api";

import type { RegisterCustomerRequest } from "../model";

export const createCustomer = async (data: RegisterCustomerRequest): Promise<string> => {
	return apiClient.post<string>("/customers/register", data);
};
