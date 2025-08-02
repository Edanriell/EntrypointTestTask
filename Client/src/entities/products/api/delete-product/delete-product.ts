import { apiClient } from "@shared/api";

export const deleteUser = async (productId: string): Promise<void> => {
	return apiClient.delete<void>(`/products/${productId}`);
};
