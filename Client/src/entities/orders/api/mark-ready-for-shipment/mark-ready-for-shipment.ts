import { apiClient } from "@shared/api";

import { MarkReadyForShipmentCommand } from "./mark-ready-for-shipment-command";

export const markReadyForShipment = async ({
	orderId
}: MarkReadyForShipmentCommand): Promise<void> => {
	return apiClient.patch<void>(`/orders/${orderId}/ready-for-shipment`);
};
