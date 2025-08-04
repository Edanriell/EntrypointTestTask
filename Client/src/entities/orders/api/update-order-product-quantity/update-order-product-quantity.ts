import { apiClient } from "@shared/api";

import { UpdateOrderProductQuantityCommand } from "./update-order-product-quantity-command";

export const updateOrderProductQuantity = async ({
	orderId,
	productId,
	productData
}: UpdateOrderProductQuantityCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/products/${productId}/quantity`);
};
