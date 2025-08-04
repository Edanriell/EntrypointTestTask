import { apiClient } from "@shared/api";

import { MarkOrderAsDeliveredCommand } from "./mark-order-as-delivered-command";

export const markOrderAsDelivered = async ({
	orderId
}: MarkOrderAsDeliveredCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/deliver`);
};
