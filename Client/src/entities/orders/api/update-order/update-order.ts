import { apiClient } from "@shared/api";

import { UpdateOrderCommand } from "./update-order-command";

export const updateOrder = async ({
	orderId,
	updatedOrderData
}: UpdateOrderCommand): Promise<void> => {
	return apiClient.put<void>(`orders/${orderId}`, updatedOrderData);
};
