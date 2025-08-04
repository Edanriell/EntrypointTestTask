import { apiClient } from "@shared/api";

import { RefundPaymentsCommand } from "./refund-payments-command";

export const refundPayments = async ({
	orderId,
	refundReason
}: RefundPaymentsCommand): Promise<void> => {
	return apiClient.post<void>(`/payments/order/${orderId}/refund`, { refundReason });
};
