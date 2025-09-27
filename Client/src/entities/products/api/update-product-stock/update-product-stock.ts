import { apiClient } from "@shared/api";

import type { UpdateProductStockCommand } from "./update-product-stock-command";

export const updateProductStock = async ({
	productId,
	updatedProductStockData
}: UpdateProductStockCommand): Promise<void> => {
	// API changed a bit
	return apiClient.patch<void>(`/products/${productId}/stock`, {
		stock: updatedProductStockData.totalStock
	});
};
