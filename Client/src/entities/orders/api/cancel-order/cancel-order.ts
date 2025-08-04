import { apiClient } from "@shared/api";

import { CancelOrderCommand } from "./cancel-order-command";

export const cancelOrder = async ({
	orderId,
	cancellationReason
}: CancelOrderCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/cancel`, { cancellationReason });
};
