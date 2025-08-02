import { apiClient } from "@shared/api";

import type { RegisterCustomerCommand } from "./register-customer-command";

export const registerCustomer = async (data: RegisterCustomerCommand): Promise<string> => {
	return apiClient.post<string>("/customers/register", data);
};
