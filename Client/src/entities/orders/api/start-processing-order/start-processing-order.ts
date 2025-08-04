import { apiClient } from "@shared/api";

import { StartProcessingOrderCommand } from "./start-processing-order-command";

export const startProcessingOrder = async ({
	orderId
}: StartProcessingOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/start-processing`);
};
