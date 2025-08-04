import { apiClient } from "@shared/api";

import { ProcessPaymentCommand } from "./process-payment-command";

export const processPayment = async ({ paymentId }: ProcessPaymentCommand): Promise<void> => {
	return apiClient.patch<void>(`/payments/${paymentId}/process`);
};
