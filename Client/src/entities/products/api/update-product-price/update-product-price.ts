import { apiClient } from "@shared/api";

import type { UpdateProductPriceCommand } from "./update-product-price-command";

export const updateProductPrice = async ({
	productId,
	updatedProductPriceData
}: UpdateProductPriceCommand): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/price`, updatedProductPriceData);
};
