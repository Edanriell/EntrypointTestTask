import { apiClient } from "@shared/api";

import { ReturnOrderCommand } from "./return-order-command";

export const returnOrder = async ({ orderId, returnReason }: ReturnOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/return`, { returnReason });
};
