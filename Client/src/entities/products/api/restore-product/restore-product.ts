import { apiClient } from "@shared/api";

import { RestoreProductCommand } from "./restore-product-command";

export const restoreProduct = async ({ productId }: RestoreProductCommand): Promise<void> => {
	return apiClient.post<void>(`/products/${productId}/restore`);
};
