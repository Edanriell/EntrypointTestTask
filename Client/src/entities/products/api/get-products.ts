import { apiClient } from "@shared/api";

import { Product, ProductsListParams, ProductsListResponse } from "../model";

export const getProducts = async (params?: ProductsListParams): Promise<ProductsListResponse> => {
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

	return await apiClient.get<ProductsListResponse>(url);
};

export const getProductById = async (productId: string): Promise<Product> => {
	return apiClient.get<Product>(`/products/${productId}`);
};
