import { apiClient } from "@shared/api";

import { GetCustomersQuery } from "./get-customers-query";
import { GetCustomersResponse } from "./get-customers-response";

export const getCustomers = async (params?: GetCustomersQuery): Promise<GetCustomersResponse> => {
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

	return await apiClient.get<GetCustomersResponse>(url);
};
