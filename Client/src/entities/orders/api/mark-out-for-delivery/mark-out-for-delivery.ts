import { apiClient } from "@shared/api";

import { MarkOutForDeliveryCommand } from "./mark-out-for-delivery-command";

export const markOutForDelivery = async ({
	orderId,
	estimatedDeliveryDate
}: MarkOutForDeliveryCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/out-for-delivery`, { estimatedDeliveryDate });
};
