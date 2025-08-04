import { apiClient } from "@shared/api";

import type { DeleteProductCommand } from "./delete-product-command";

export const deleteProduct = async ({ productId }: DeleteProductCommand): Promise<void> => {
	return apiClient.delete<void>(`/products/${productId}`);
};
