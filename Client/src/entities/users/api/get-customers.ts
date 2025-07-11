import { apiClient } from "@shared/api";

import { Customer, CustomersListParams, CustomersListResponse } from "@entities/users/model/types";

export const getCustomers = async (
	params?: CustomersListParams
): Promise<CustomersListResponse> => {
	// Build query string from parameters
	const queryParams = new URLSearchParams();

	if (params) {
		Object.entries(params).forEach(([key, value]) => {
			if (value !== undefined && value !== null && value !== "") {
				queryParams.append(key, value.toString());
			}
		});
	}

	const queryString = queryParams.toString();
	const url = `/customers${queryString ? `?${queryString}` : ""}`;

	return await apiClient.get<CustomersListResponse>(url);
};

export const getCustomerById = async (id: string): Promise<Customer> => {
	return apiClient.get<Customer>(`/customers/${id}`);
};
