import { apiClient } from "@shared/api";

import type { RemoveProductFromOrderCommand } from "./remove-product-from-order-command";

export const removeProductFromOrder = async ({
	orderId,
	productRemovals
}: RemoveProductFromOrderCommand): Promise<void> => {
	return apiClient.delete<void>(`/orders/${orderId}/products`, productRemovals);
};
