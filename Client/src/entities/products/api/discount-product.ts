import { apiClient } from "@shared/api";

import { DiscountProductRequest } from "../model";

export const discountProduct = async (
	productId: string,
	productData: DiscountProductRequest
): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/discount`, productData);
};
