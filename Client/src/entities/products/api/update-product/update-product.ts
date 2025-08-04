import { apiClient } from "@shared/api";

import { UpdateProductCommand } from "./update-product-command";

export const updateProduct = async ({
	productId,
	updatedProductData
}: UpdateProductCommand): Promise<void> => {
	return apiClient.put<void>(`/products/${productId}`, updatedProductData);
};
