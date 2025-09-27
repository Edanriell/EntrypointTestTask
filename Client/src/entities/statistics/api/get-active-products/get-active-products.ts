import { apiClient } from "@shared/api";

import { GetActiveProductsResponse } from "./get-active-products-response";

export const getActiveProducts = async (): Promise<GetActiveProductsResponse> => {
	return await apiClient.get<GetActiveProductsResponse>("/statistics/products/active");
};
