import { apiClient } from "@shared/api";

import { DiscountProductCommand } from "./discount-product-command";

export const discountProduct = async ({
	productId,
	updatedProductPriceData
}: DiscountProductCommand): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/discount`, updatedProductPriceData);
};
