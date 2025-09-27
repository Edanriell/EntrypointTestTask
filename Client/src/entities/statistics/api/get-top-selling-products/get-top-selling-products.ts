import { apiClient } from "@shared/api";

import { GetTopSellingProductsResponse } from "./get-top-selling-products-response";

export const getTopSellingProducts = async (): Promise<GetTopSellingProductsResponse> => {
	return await apiClient.get<GetTopSellingProductsResponse>("/statistics/products/top-selling");
};
