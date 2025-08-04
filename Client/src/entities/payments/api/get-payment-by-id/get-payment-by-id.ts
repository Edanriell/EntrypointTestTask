import { apiClient } from "@shared/api";

import { GetPaymentByIdResponse } from "./get-payment-by-id-response";
import { GetPaymentByIdQuery } from "./get-payment-by-id-query";

export const getPaymentById = async ({
	paymentId
}: GetPaymentByIdQuery): Promise<GetPaymentByIdResponse> => {
	return apiClient.get<GetPaymentByIdResponse>(`/payments/${paymentId}`);
};
