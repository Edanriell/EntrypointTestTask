import { apiClient } from "@shared/api";

import type { RegisterCustomerCommand } from "./register-customer-command";
import type { RegisterCustomerResponse } from "./register-customer-response";

export const registerCustomer = async (
	data: RegisterCustomerCommand
): Promise<RegisterCustomerResponse> => {
	return apiClient.post<RegisterCustomerResponse>("/customers/register", data);
};
