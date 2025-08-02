import { apiClient } from "@shared/api";

export const restoreProduct = async (productId: string): Promise<void> => {
	return apiClient.post<void>(`/products/${productId}/restore`);
};
