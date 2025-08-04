import { apiClient } from "@shared/api";

import { ConfirmOrderCommand } from "./confirm-order-command";

export const confirmOrder = async ({ orderId }: ConfirmOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/confirm`);
};
