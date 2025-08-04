import { apiClient } from "@shared/api";

import type { GetOrdersQuery } from "./get-orders-query";
import type { GetOrdersResponse } from "./get-orders-response";

export const getOrders = async (params?: GetOrdersQuery): Promise<GetOrdersResponse> => {
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
	const url = `/orders${queryString ? `?${queryString}` : ""}`;

	return await apiClient.get<GetOrdersResponse>(url);
};
