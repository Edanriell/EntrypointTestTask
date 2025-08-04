import { apiClient } from "@shared/api";

import type { GetProductsQuery } from "./get-products-query";
import type { GetProductsResponse } from "./get-products-response";

export const getProducts = async (params?: GetProductsQuery): Promise<GetProductsResponse> => {
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
	const url = `/products${queryString ? `?${queryString}` : ""}`;

	return await apiClient.get<GetProductsResponse>(url);
};
