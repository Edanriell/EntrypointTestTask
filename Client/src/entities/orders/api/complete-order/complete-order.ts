import { apiClient } from "@shared/api";

import { CompleteOrderCommand } from "./complete-order-command";

export const completeOrder = async ({ orderId }: CompleteOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/complete`);
};
