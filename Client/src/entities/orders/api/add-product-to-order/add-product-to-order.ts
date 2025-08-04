import { apiClient } from "@shared/api";

import { AddProductToOrderCommand } from "./add-product-to-order-command";

export const addProductToOrder = async ({
	orderId,
	products
}: AddProductToOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/products`, products);
};
