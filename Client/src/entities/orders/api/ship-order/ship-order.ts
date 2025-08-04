import { apiClient } from "@shared/api";

import { ShipOrderCommand } from "./ship-order-command";

export const shipOrder = async ({ orderId, orderData }: ShipOrderCommand): Promise<void> => {
	console.log(orderData);
	return apiClient.patch<void>(`/orders/${orderId}/ship`, orderData);
};
