import { apiClient } from "@shared/api";

import type { DeleteOrderCommand } from "./delete-order-command";

export const deleteOrder = async ({ orderId }: DeleteOrderCommand): Promise<void> => {
	return apiClient.delete<void>(`/orders/${orderId}`);
};
