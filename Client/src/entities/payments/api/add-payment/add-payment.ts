import { apiClient } from "@shared/api";

import type { AddPaymentCommand } from "./add-payment-command";
import type { AddPaymentResponse } from "./add-payment-response";

export const addPayment = async (data: AddPaymentCommand): Promise<AddPaymentResponse> => {
	return apiClient.post<AddPaymentResponse>("/payments", data);
};
