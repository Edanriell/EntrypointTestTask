import { apiClient } from "@shared/api";

import { Customer } from "../model";

export const getCustomerById = async (id: string): Promise<Customer> => {
	// ISSUE HERE GetCustomerById Response !
	return apiClient.get<Customer>(`/customers/${id}`);
};
