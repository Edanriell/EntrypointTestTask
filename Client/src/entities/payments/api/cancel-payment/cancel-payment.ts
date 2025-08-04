import { apiClient } from "@shared/api";

import { CancelPaymentCommand } from "./cancel-payment-command";

export const cancelPayment = async ({ paymentId }: CancelPaymentCommand): Promise<void> => {
	return apiClient.patch<void>(`/payments/${paymentId}/cancel`);
};
