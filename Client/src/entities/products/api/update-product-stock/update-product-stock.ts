import { apiClient } from "@shared/api";

import type { UpdateProductStockCommand } from "./update-product-stock-command";

export const updateProductStock = async ({
	productId,
	updatedProductStockData
}: UpdateProductStockCommand): Promise<void> => {
	return apiClient.patch<void>(`/products/${productId}/stock`, updatedProductStockData);
};
